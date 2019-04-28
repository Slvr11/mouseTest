using System;
using System.Collections.Generic;
using System.Collections;
using InfinityScript;
using static InfinityScript.GSCFunctions;

public class menuTest : BaseScript
{
    public menuTest()
    {
        PreCacheShader("ui_cursor");
        PreCacheShader("hud_iw5_killstreak_box");
        
        PlayerConnected += onPlayerConnect;
        //Build main menu
        addButton(0, 0, 192, 64, "Test Button 1", testButtonFunc);
        addButton(0, 128, 192, 64, "Toggle Noclip", noClip);
        addButton(0, -128, 192, 64, "Godmode", godmode);
    }

    public static void onPlayerConnect(Entity player)
    {
        player.SpawnedPlayer += () => onPlayerSpawn(player);
        player.SetField("cursorPos", Vector3.Zero);
        player.SetField("lastCursorPos", Vector3.Zero);
        player.SetField("lastAngles", Vector3.Zero);
        player.SetField("originalAngles", Vector3.Zero);
        player.SetField("menuOpen", false);
        player.SetField("menu", new Parameter(new List<HudElem>()));

        HudElem cursor = NewClientHudElem(player);
        cursor.AlignX = HudElem.XAlignments.Center;
        cursor.AlignY = HudElem.YAlignments.Middle;
        cursor.Alpha = 0;
        cursor.Archived = true;
        cursor.Foreground = true;
        cursor.HideIn3rdPerson = false;
        cursor.HideWhenDead = false;
        cursor.HideWhenInDemo = false;
        cursor.HideWhenInMenu = true;
        cursor.LowResBackground = false;
        cursor.HorzAlign = HudElem.HorzAlignments.Center_Adjustable;
        cursor.VertAlign = HudElem.VertAlignments.Middle;
        cursor.X = 0;
        cursor.Y = 0;
        cursor.Sort = 10000;
        cursor.SetShader("ui_cursor", 32, 32);
        player.SetField("cursor", cursor);


        player.NotifyOnPlayerCommand("openTestMenu", "vote yes");
        player.NotifyOnPlayerCommand("click", "+attack");

        player.OnNotify("openTestMenu", openTestMenu);
        player.OnNotify("click", executeClick);
    }

    public static void onPlayerSpawn(Entity player)
    {
        player.SetField("lastAngles", player.GetPlayerAngles());
    }

    private static void giveMaxAmmo(Entity player)
    {
        player.EnableWeapons();
        StartAsync(giveAmmoUponChange(player));
        player.PlayLocalSound("ammo_crate_use");
        player.ClientPrint("MP_DEPLOYED_AMMO");
    }
    private static IEnumerator giveAmmoUponChange(Entity player)
    {
        while (player.CurrentWeapon == "none")
            yield return WaitForFrame();

        player.GiveMaxAmmo(player.CurrentWeapon);
        player.DisableWeapons();
    }
    private static void godmode(Entity player)
    {
        player.Health = 999999;
        player.ClientPrint("GAME_GODMODE_ON");
    }
    private static void testButtonFunc(Entity player)
    {
        player.IPrintLnBold("^2You clicked the Test Button!!");
    }

    private static void noClip(Entity player)
    {
        if (!player.HasField("isInNoClip")) player.SetField("isInNoClip", false);

        player.SetField("isInNoClip", !player.GetField<bool>("isInNoClip"));

        byte set = (byte)(player.GetField<bool>("isInNoClip") ? 0x01 : 0x00);
        unsafe
        {
            *(byte*)(0x01AC56C0 + (player.EntRef * 0x38A4)) = set;
        }

        string value = player.GetField<bool>("isInNoClip") ? "ON" : "OFF";
        player.ClientPrint("GAME_NOCLIP" + value);
    }

    public static void openTestMenu(Entity player)
    {
        if (player.GetField<bool>("menuOpen"))
        {
            closeTestMenu(player);
            return;
        }

        player.AllowAds(false);
        player.AllowJump(false);
        player.AllowSprint(false);
        player.SetMoveSpeedScale(0);
        player.DisableOffhandWeapons();
        player.DisableUsability();
        player.DisableWeaponPickup();
        player.DisableWeapons();
        player.DisableWeaponSwitch();
        //player.SetClientDvar("ui_drawCrosshair", "0");
        player.SetClientDvar("g_hardcore", "1");
        player.SetClientDvar("g_scriptMainMenu", "a");
        player.SetField("originalAngles", player.GetPlayerAngles());
        player.SetField("cursorPos", Vector3.Zero);
        player.SetField("lastCursorPos", Vector3.Zero);
        player.SetField("lastAngles", Vector3.Zero);

        HudElem cursor = player.GetField<HudElem>("cursor");
        cursor.FadeOverTime(.5f);
        cursor.Alpha = 1;

        //Opening main menu
        player.SetField("menuOpen", true);
        List<HudElem> menu = player.GetField<List<HudElem>>("menu");
        HudElem bg = NewClientHudElem(player);
        bg.AlignX = HudElem.XAlignments.Left;
        bg.AlignY = HudElem.YAlignments.Top;
        bg.Alpha = 0;
        bg.FadeOverTime(.5f);
        bg.Alpha = 1;
        bg.Color = Vector3.Zero;
        bg.Foreground = false;
        bg.HideIn3rdPerson = false;
        bg.HideWhenDead = false;
        bg.HideWhenInDemo = false;
        bg.HideWhenInMenu = false;
        bg.HorzAlign = HudElem.HorzAlignments.Fullscreen;
        bg.VertAlign = HudElem.VertAlignments.Fullscreen;
        bg.LowResBackground = false;
        bg.X = 0;
        bg.Y = 0;
        bg.Sort = 0;
        bg.SetShader("white", 640, 480);
        menu.Add(bg);

        HudElem menuTitle = NewClientHudElem(player);
        menuTitle.AlignX = HudElem.XAlignments.Center;
        menuTitle.AlignY = HudElem.YAlignments.Top;
        menuTitle.Alpha = 0;
        menuTitle.FadeOverTime(.5f);
        menuTitle.Alpha = 1;
        menuTitle.Color = new Vector3(0, 1, 0);
        menuTitle.Foreground = false;
        menuTitle.HideIn3rdPerson = false;
        menuTitle.HideWhenDead = false;
        menuTitle.HideWhenInDemo = false;
        menuTitle.HideWhenInMenu = false;
        menuTitle.HorzAlign = HudElem.HorzAlignments.Center_Adjustable;
        menuTitle.VertAlign = HudElem.VertAlignments.Top_Adjustable;
        menuTitle.X = 0;
        menuTitle.Y = 15;
        menuTitle.Sort = 1;
        menuTitle.Font = HudElem.Fonts.HudBig;
        menuTitle.FontScale = 1;
        menuTitle.SetText("TEST MENU");
        menu.Add(menuTitle);

        foreach (Button button in Button.menuButtons)
        {
            HudElem buttonBG = NewClientHudElem(player);
            buttonBG.AlignX = HudElem.XAlignments.Center;
            buttonBG.AlignY = HudElem.YAlignments.Middle;
            buttonBG.Alpha = 0;
            buttonBG.FadeOverTime(.5f);
            buttonBG.Alpha = 1;
            buttonBG.Color = new Vector3(1, 1, 1);
            buttonBG.Foreground = false;
            buttonBG.HideIn3rdPerson = false;
            buttonBG.HideWhenDead = false;
            buttonBG.HideWhenInDemo = false;
            buttonBG.HideWhenInMenu = false;
            buttonBG.HorzAlign = HudElem.HorzAlignments.Center_Adjustable;
            buttonBG.VertAlign = HudElem.VertAlignments.Middle;
            buttonBG.LowResBackground = false;
            buttonBG.X = button.X;
            buttonBG.Y = button.Y;
            buttonBG.Sort = 2;
            buttonBG.SetShader("hud_iw5_killstreak_box", button.Width, button.Height);
            menu.Add(buttonBG);

            HudElem buttonText = HudElem.CreateFontString(player, HudElem.Fonts.HudSmall, (button.Width + button.Height) / 256);
            buttonText.Parent = buttonBG;
            buttonText.SetPoint("center", "center");
            buttonText.SetText(button.Text);
            buttonText.Alpha = 0;
            buttonText.FadeOverTime(.5f);
            buttonText.Alpha = 1;
            menu.Add(buttonText);

            button.hud = buttonBG;
        }

        player.SetField("menu", new Parameter(menu));

        OnInterval(50, () => trackMouseMovement(player));
    }

    private static void closeTestMenu(Entity player)
    {
        player.AllowAds(true);
        player.AllowJump(true);
        player.AllowSprint(true);
        player.SetMoveSpeedScale(1);
        player.EnableOffhandWeapons();
        player.EnableUsability();
        player.EnableWeaponPickup();
        player.EnableWeapons();
        player.EnableWeaponSwitch();
        //player.SetClientDvar("ui_drawCrosshair", "1");
        player.SetClientDvar("g_hardcore", "0");
        player.SetClientDvar("g_scriptMainMenu", "class");
        player.ClearField("hoveredButton");

        HudElem cursor = player.GetField<HudElem>("cursor");
        cursor.FadeOverTime(.25f);
        cursor.Alpha = 0;

        foreach (HudElem hud in player.GetField<List<HudElem>>("menu"))
        {
            hud.FadeOverTime(.25f);
            hud.Alpha = 0;
            AfterDelay(250, () => hud.Destroy());
        }
        foreach (Button button in Button.menuButtons)
        {
            button.hud = null;
            button.isHovered = false;
        }

        player.SetField("menuOpen", false);
    }

    public static void addButton(int x, int y, int width, int height, string text, Action<Entity> action)
    {
        Button newButton = new Button(x, y, width, height, text, action);
    }

    private static void executeClick(Entity player)
    {
        if (!player.GetField<bool>("menuOpen") || !player.HasField("hoveredButton")) return;

        player.PlayLocalSound("mouse_click");
        player.GetField<Button>("hoveredButton").onClick(player);
    }

    private static bool isCursorHoveringButton(Entity player, Button button)
    {
        HudElem cursor = player.GetField<HudElem>("cursor");
        float cursorX = cursor.X;
        float cursorY = cursor.Y;

        if (cursorX - 360 > (button.X + (button.Width / 2))) return false;
        if (cursorX - 360 < (button.X - (button.Width / 2))) return false;
        if (cursorY > (button.Y + (button.Height / 2))) return false;
        if (cursorY < (button.Y - (button.Height / 2))) return false;

        return true;
    }

    private static bool trackMouseMovement(Entity player)
    {
        HudElem cursor = player.GetField<HudElem>("cursor");
        Vector3 lastPos = player.GetField<Vector3>("lastCursorPos");
        Vector3 lastAngles = player.GetField<Vector3>("lastAngles");
        Vector3 newAngles = player.GetPlayerAngles() * 2;
        Vector3 newCursorPos = lastPos + (newAngles - lastAngles);
        newCursorPos.Z = 0;
        if (newCursorPos.X > 640) newCursorPos.X = 640;
        if (newCursorPos.Y > 480) newCursorPos.Y = 480;

        cursor.MoveOverTime(.05f);
        cursor.X = -newCursorPos.Y + 360;
        cursor.Y = newCursorPos.X;
        player.SetField("lastAngles", newAngles);
        player.SetField("lastCursorPos", newCursorPos);

        if (!player.HasField("hoveredButton"))
        {
            foreach (Button button in Button.menuButtons)
            {
                if (button.isHovered || button.hud == null) continue;

                if (isCursorHoveringButton(player, button))
                {
                    button.onHover();
                    player.PlayLocalSound("mouse_over");
                    player.SetField("hoveredButton", new Parameter(button));
                    OnInterval(50, () => watchCursorHoverOnButton(player, button));
                }
            }
        }

        if (!player.GetField<bool>("menuOpen")) return false;
        return true;
    }

    private static bool watchCursorHoverOnButton(Entity player, Button button)
    {
        if (!isCursorHoveringButton(player, button))
        {
            player.ClearField("hoveredButton");
            button.onExitHover();
            return false;
        }

        return true;
    }
}
public class Button
{
    public static List<Button> menuButtons = new List<Button>();
    public int X;
    public int Y;
    public int Width;
    public int Height;
    public string Text = "";
    public Action<Entity> onClick;
    public HudElem hud = null;
    public bool isHovered = false;
    //public string VisibleState = "main";//For multiple menus

    public Button(int x, int y, int width, int height, string text, Action<Entity> action)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Text = text;
        onClick = action;

        menuButtons.Add(this);
    }

    public void onHover()
    {
        if (hud == null)
            return;

        if (isHovered)
            return;

        hud.ScaleOverTime(.25f, Width + 12, Height + 12);
        hud.Color = new Vector3(1, 1, 0);
        hud.Children[0].ChangeFontScaleOverTime(.25f);
        hud.Children[0].FontScale += .2f;
        isHovered = true;
    }
    public void onExitHover()
    {
        if (hud == null)
            return;

        if (!isHovered)
            return;

        hud.ScaleOverTime(.25f, Width, Height);
        hud.Color = new Vector3(1, 1, 1);
        hud.Children[0].ChangeFontScaleOverTime(.25f);
        hud.Children[0].FontScale -= .2f;
        isHovered = false;
    }
}

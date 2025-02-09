﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using static SCJMapper_V2.Layout.MapProps;

namespace SCJMapper_V2
{
  sealed class AppSettings : ApplicationSettingsBase, IDisposable
  {
    FormSettings FS = null; // Settings form


    // Singleton
    private static readonly Lazy<AppSettings> m_lazy = new Lazy<AppSettings>( () => new AppSettings( ) );
    public static AppSettings Instance { get => m_lazy.Value; }

    private AppSettings()
    {
      if ( this.FirstRun ) {
        // migrate the settings to the new version if the app runs the first time
        try {
          this.Upgrade( );
        }
        catch { }
        this.FirstRun = false;
        this.Save( );
      }
      if ( string.IsNullOrEmpty( UseLanguage ) ) {
        UseLanguage = SC.SCUiText.Languages.profile.ToString( ); // get a default here
        this.Save( );
      }
    }

    public void Dispose( bool disposing )
    {
      if ( disposing ) {
        // dispose managed resources
        if ( FS != null ) FS.Dispose( );
      }
      // free native resources
    }

    public void Dispose()
    {
      Dispose( true );
      GC.SuppressFinalize( this );
    }


    /// <summary>
    /// Show the Settings Dialog
    /// </summary>
    public DialogResult ShowSettings( string pasteString )
    {
      if ( FS == null ) FS = new FormSettings( );
      FS.PasteString = pasteString; // propagate joyinput
      FS.ShowDialog( );
      return ( FS.Canceled ) ? DialogResult.Cancel : DialogResult.OK;
    }


    #region Setting Properties

    // manages Upgrade
    [UserScopedSetting( )]
    [DefaultSettingValue( "True" )]
    public bool FirstRun
    {
      get { return (bool)this["FirstRun"]; }
      set { this["FirstRun"] = value; }
    }


    // Control bound settings
    [UserScopedSetting( )]
    [DefaultSettingValue( "1000, 900" )]
    public Size FormSize
    {
      get { return (Size)this["FormSize"]; }
      set { this["FormSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "10, 10" )]
    public Point FormLocation
    {
      get { return (Point)this["FormLocation"]; }
      set { this["FormLocation"] = value; }
    }

    // User Config Settings
    [UserScopedSetting( )]
    [DefaultSettingValue( "layout_joystick_spacesim" )] // from Game Bundle
    public string DefMappingName
    {
      get { return (string)this["DefMappingName"]; }
      set { this["DefMappingName"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "layout_my_joystick" )] // just a default
    public string MyMappingName
    {
      get { return (string)this["MyMappingName"]; }
      set { this["MyMappingName"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "True" )]
    public bool ShowJoystick
    {
      get { return (bool)this["ShowJoystick"]; }
      set { this["ShowJoystick"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "True" )]
    public bool ShowGamepad
    {
      get { return (bool)this["ShowGamepad"]; }
      set { this["ShowGamepad"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "True" )]
    public bool ShowKeyboard
    {
      get { return (bool)this["ShowKeyboard"]; }
      set { this["ShowKeyboard"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "True" )]
    public bool ShowMouse  // 20151220BM: add mouse device (from AC 2.0 defaultProfile usage)
    {
      get { return (bool)this["ShowMouse"]; }
      set { this["ShowMouse"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool ShowMapped
    {
      get { return (bool)this["ShowMapped"]; }
      set { this["ShowMapped"] = value; }
    }


    // Settings Window

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS1
    {
      get { return (string)this["IgnoreJS1"]; }
      set { this["IgnoreJS1"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS2
    {
      get { return (string)this["IgnoreJS2"]; }
      set { this["IgnoreJS2"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS3
    {
      get { return (string)this["IgnoreJS3"]; }
      set { this["IgnoreJS3"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS4
    {
      get { return (string)this["IgnoreJS4"]; }
      set { this["IgnoreJS4"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS5
    {
      get { return (string)this["IgnoreJS5"]; }
      set { this["IgnoreJS5"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS6
    {
      get { return (string)this["IgnoreJS6"]; }
      set { this["IgnoreJS6"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS7
    {
      get { return (string)this["IgnoreJS7"]; }
      set { this["IgnoreJS7"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS8
    {
      get { return (string)this["IgnoreJS8"]; }
      set { this["IgnoreJS8"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS9
    {
      get { return (string)this["IgnoreJS9"]; }
      set { this["IgnoreJS9"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS10
    {
      get { return (string)this["IgnoreJS10"]; }
      set { this["IgnoreJS10"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS11
    {
      get { return (string)this["IgnoreJS11"]; }
      set { this["IgnoreJS11"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string IgnoreJS12
    {
      get { return (string)this["IgnoreJS12"]; }
      set { this["IgnoreJS12"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string JSnHide
    {
      get { return (string)this["JSnHide"]; }
      set { this["JSnHide"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string JSnColor
    {
      get { return (string)this["JSnColor"]; }
      set { this["JSnColor"] = value; }
    }



    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string UserSCPath
    {
      get { return (string)this["UserSCPath"]; }
      set { this["UserSCPath"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool UserSCPathUsed
    {
      get { return (bool)this["UserSCPathUsed"]; }
      set { this["UserSCPathUsed"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( ",default,multiplayer,singleplayer,player,flycam,vehicle_driver," )] // empty  Note: comma separated list, must have a comma at the begining and the end (to find 'player' on its own...)
    public string IgnoreActionmaps
    {
      get { return (string)this["IgnoreActionmaps"]; }
      set { this["IgnoreActionmaps"] = value; }
    }


    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool DetectGamepad
    {
      get { return (bool)this["DetectGamepad"]; }
      set { this["DetectGamepad"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool UsePTU
    {
      get { return (bool)this["UsePTU"]; }
      set { this["UsePTU"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool UseCSVListing
    {
      get { return (bool)this["UseCSVListing"]; }
      set { this["UseCSVListing"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool ListModifiers
    {
      get { return (bool)this["ListModifiers"]; }
      set { this["ListModifiers"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool AutoTabXML
    {
      get { return (bool)this["AutoTabXML"]; }
      set { this["AutoTabXML"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "profile" )]
    public string UseLanguage
    {
      get { return (string)this["UseLanguage"]; }
      set { this["UseLanguage"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool ShowTreeTips
    {
      get { return (bool)this["ShowTreeTips"]; }
      set { this["ShowTreeTips"] = value; }
    }



    //**** Form Table

    // Control bound settings
    [UserScopedSetting( )]
    [DefaultSettingValue( "1000, 900" )]
    public Size FormTableSize
    {
      get { return (Size)this["FormTableSize"]; }
      set { this["FormTableSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "760, 320" )]
    public Point FormTableLocation
    {
      get { return (Point)this["FormTableLocation"]; }
      set { this["FormTableLocation"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string FormTableColumnWidth
    {
      get { return (string)this["FormTableColumnWidth"]; }
      set { this["FormTableColumnWidth"] = value; }
    }


    //**** Form Options

    [UserScopedSetting( )]
    [DefaultSettingValue( "1000, 765" )]
    public Size FormOptionsSize
    {
      get { return (Size)this["FormOptionsSize"]; }
      set { this["FormOptionsSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "10, 10" )]
    public Point FormOptionsLocation
    {
      get { return (Point)this["FormOptionsLocation"]; }
      set { this["FormOptionsLocation"] = value; }
    }


    //**** Form Layout

    [UserScopedSetting( )]
    [DefaultSettingValue( "1000, 765" )]
    public Size FormLayoutSize
    {
      get { return (Size)this["FormLayoutSize"]; }
      set { this["FormLayoutSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "10, 10" )]
    public Point FormLayoutLocation
    {
      get { return (Point)this["FormLayoutLocation"]; }
      set { this["FormLayoutLocation"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "16" )]
    public int LayoutFontSize
    {
      get { return (int)this["LayoutFontSize"]; }
      set { this["LayoutFontSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,0,0,139|255,255,255,255" )]
    public string GroupColor_00
    {
      get { return (string)this["GroupColor_00"]; }
      set { this["GroupColor_00"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,255,140,00|255,255,255,255" )]
    public string GroupColor_01
    {
      get { return (string)this["GroupColor_01"]; }
      set { this["GroupColor_01"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,138,43,226|255,255,255,255" )]
    public string GroupColor_02
    {
      get { return (string)this["GroupColor_02"]; }
      set { this["GroupColor_02"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,220,20,60|255,255,255,255" )]
    public string GroupColor_03
    {
      get { return (string)this["GroupColor_03"]; }
      set { this["GroupColor_03"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,184,134,11|255,255,255,255" )]
    public string GroupColor_04
    {
      get { return (string)this["GroupColor_04"]; }
      set { this["GroupColor_04"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,46,139,87|255,255,255,255" )]
    public string GroupColor_05
    {
      get { return (string)this["GroupColor_05"]; }
      set { this["GroupColor_05"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,128,128,0|255,255,255,255" )]
    public string GroupColor_06
    {
      get { return (string)this["GroupColor_06"]; }
      set { this["GroupColor_06"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,47,79,79|255,255,255,255" )]
    public string GroupColor_07
    {
      get { return (string)this["GroupColor_07"]; }
      set { this["GroupColor_07"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,255,0,0|255,255,255,255" )]
    public string GroupColor_08
    {
      get { return (string)this["GroupColor_08"]; }
      set { this["GroupColor_08"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,255,215,0|255,255,255,255" )]
    public string GroupColor_09
    {
      get { return (string)this["GroupColor_09"]; }
      set { this["GroupColor_09"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0|255,255,255,255" )]
    public string GroupColor_10
    {
      get { return (string)this["GroupColor_10"]; }
      set { this["GroupColor_10"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,128,0,128|255,255,255,255" )]
    public string GroupColor_11
    {
      get { return (string)this["GroupColor_11"]; }
      set { this["GroupColor_11"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "255,255,20,147|255,255,255,255" )]
    public string GroupColor_12
    {
      get { return (string)this["GroupColor_12"]; }
      set { this["GroupColor_12"] = value; }
    }


    #endregion


  }
}

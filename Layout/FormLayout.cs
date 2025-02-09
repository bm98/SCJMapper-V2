﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SCJMapper_V2.Common;
using SCJMapper_V2.Translation;
using SCJMapper_V2.Actions;
using SCJMapper_V2.SC;
using static SCJMapper_V2.Layout.ActionGroups;
using System.IO;

namespace SCJMapper_V2.Layout
{
  public partial class FormLayout : Form
  {
    // logger
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Graphics
    private DrawPanel m_dPanel = new DrawPanel( ); // draw panel
    private DrawPanel m_sPanel = new DrawPanel( ); // show panel
    private DisplayList m_displayList = new DisplayList( );
    private readonly Color COL_OK = Color.LightGreen;
    private readonly Color COL_ERR = Color.Gold;

    private Layouts m_layouts = null;
    private List<Device.DeviceDescriptor> m_devices = null; // for debug allocated only

    internal ActionItemList ActionList { get; set; } = null;

    private bool IsDebug()
    {
      return File.Exists( "DEBUG_LAYOUT.txt" );
    }

    private bool m_checkAllToggle = true;
    private void CheckAllGroups()
    {
      // select all groups
      for ( int i = 0; i < chkLbActionGroups.Items.Count; i++ ) {
        chkLbActionGroups.Items[i].Checked = m_checkAllToggle;
      }
      m_checkAllToggle = !m_checkAllToggle; // toggle
    }

    #region JsCombo stuff

    private void LoadJsCombos()
    {

      cbxJs1.Items.Clear( ); cbxJs2.Items.Clear( ); cbxJs3.Items.Clear( ); cbxJs4.Items.Clear( );
      cbxJs1.Items.Add( new Device.DeviceDescriptor( ) { DevName = " Js1 Not Used", DevGuid = "{99999991-0000-0000-000000000000}" } );
      cbxJs2.Items.Add( new Device.DeviceDescriptor( ) { DevName = " Js2 Not Used", DevGuid = "{99999992-0000-0000-000000000000}" } );
      cbxJs3.Items.Add( new Device.DeviceDescriptor( ) { DevName = " Js3 Not Used", DevGuid = "{99999993-0000-0000-000000000000}" } );
      cbxJs4.Items.Add( new Device.DeviceDescriptor( ) { DevName = " Js4 Not Used", DevGuid = "{99999994-0000-0000-000000000000}" } );
      if ( m_devices == null ) return;

      // get all devices know in the layout folder
      foreach ( var dev in m_devices ) {
        cbxJs1.Items.Add( dev ); cbxJs2.Items.Add( dev ); cbxJs3.Items.Add( dev ); cbxJs4.Items.Add( dev );
      }
      cbxJs1.SelectedIndex = 0; cbxJs2.SelectedIndex = 0; cbxJs3.SelectedIndex = 0; cbxJs4.SelectedIndex = 0;
    }

    private int JsIndexOf(ComboBox.ObjectCollection objs, string sought )
    {
      for (int i=0; i<objs.Count; i++ ) {
        var dev = objs[i] as Device.DeviceDescriptor;
        if ( dev.DevName == sought ) {
          return i;
        }
      }
      return -1;
    }

    private void SetJsCombos(Dictionary<int,string> usedDeviceDict)
    {
      cbxJs1.SelectedIndex = 0; cbxJs2.SelectedIndex = 0; cbxJs3.SelectedIndex = 0; cbxJs4.SelectedIndex = 0;
      int i = 0;
      foreach ( var kv in usedDeviceDict ) {
        switch ( kv.Key ) {
          case 1:
            i = JsIndexOf( cbxJs1.Items, kv.Value );
            if ( i >= 0 ) cbxJs1.SelectedIndex = i;
            break;
          case 2:
            i = JsIndexOf( cbxJs2.Items, kv.Value );
            if ( i >= 0 ) cbxJs2.SelectedIndex = i;
            break;

          case 3:
            i = JsIndexOf( cbxJs3.Items, kv.Value );
            if ( i >= 0 ) cbxJs3.SelectedIndex = i;
            break;
          case 4:
            i = JsIndexOf( cbxJs4.Items, kv.Value );
            if ( i >= 0 ) cbxJs4.SelectedIndex = i;
            break;
          default: break;
        }
      }
    }

    #endregion

    /// <summary>
    /// Local to support the handling
    /// </summary>
    private class ActionMapItem
    {
      public string ActionMap = "";
      public string ActionMapDisp = "";
      public override string ToString()
      {
        return ActionMapDisp;
      }
    }

    // the item index is shared with the checkbox list in the GUI
    private List<ActionMapItem> m_chkLbActionMaps = new List<ActionMapItem>( );

    #region Main Form Handling

    public FormLayout()
    {
      InitializeComponent( );

      // Show Debug items at startup
      if ( IsDebug( ) ) {
        log.DebugFormat( "Layout - Debug Mode ON" );
        pnlDebug.Visible = true;
      }
    }

    private void FormLayout_Load( object sender, EventArgs e )
    {
      log.DebugFormat( "Layout - Form Loading" );

      Tx.LocalizeControlTree( this );

      AppSettings.Instance.Reload( );

      // Assign Size property - check if on screen, else use defaults
      if ( Commons.IsOnScreen( new Rectangle( AppSettings.Instance.FormLayoutLocation, AppSettings.Instance.FormLayoutSize ) ) ) {
        this.Size = AppSettings.Instance.FormLayoutSize;
        this.Location = AppSettings.Instance.FormLayoutLocation;
      }

      // Action Groups
      // Main dialog
      log.DebugFormat( "Layout - Form Loading - Loading Action Groups" );
      chkLbActionGroups.Items.Clear( );
      chkLbActionGroups.Columns.Add( "Actiongroups", chkLbActionMaps.Width );
      foreach ( var s in ActionGroupNames( ) ) {
        var item = chkLbActionGroups.Items.Add( s );
        item.ForeColor = MapProps.GroupColor( GroupNameToGroup( s ) ).ForeColor;
        item.BackColor = MapProps.GroupColor( GroupNameToGroup( s ) ).BackColor;
      }
      // color settings
      log.DebugFormat( "Layout - Form Loading - Loading Color Settings" );
      chkLbActionGroupsColor.Items.Clear( );
      chkLbActionGroupsColor.Columns.Add( "Actiongroups", chkLbActionMaps.Width );
      foreach ( var s in ActionGroupNames( ) ) {
        var item = chkLbActionGroupsColor.Items.Add( s );
        item.ForeColor = MapProps.GroupColor( GroupNameToGroup( s ) ).ForeColor;
        item.BackColor = MapProps.GroupColor( GroupNameToGroup( s ) ).BackColor;
      }

      // Action Maps
      log.DebugFormat( "Layout - Form Loading - Loading Action Maps" );
      chkLbActionMaps.Items.Clear( ); m_chkLbActionMaps.Clear( );
      chkLbActionMaps.Columns.Add( "Actionmaps", chkLbActionMaps.Width );
      foreach ( EGroup g in Enum.GetValues( typeof( EGroup ) ) ) {
        var aMaps = ActionmapNames( g );
        foreach ( var aMap in aMaps ) {
          if ( !AppSettings.Instance.IgnoreActionmaps.Contains( "," + aMap + "," ) ) {
            var ami = new ActionMapItem( ) {
              ActionMap = aMap,
              ActionMapDisp = SCUiText.Instance.Text( aMap )
            };
            m_chkLbActionMaps.Add( ami );
            var item = chkLbActionMaps.Items.Add( ami.ActionMapDisp );
            item.ToolTipText = ami.ActionMap;
            item.ForeColor = MapProps.MapColor( ami.ActionMap ).ForeColor;
            item.BackColor = MapProps.MapColor( ami.ActionMap ).BackColor;
          }
        }
      }

      // Layouts
      log.DebugFormat( "Layout - Form Loading - Loading Layouts" );
      m_layouts = new Layouts( );
      foreach ( var l in m_layouts ) {
        log.DebugFormat( "  + {0}", l.Filename );
        cbxLayouts.Items.Add( l );
      }
      if ( cbxLayouts.Items.Count > 0 ) {
        cbxLayouts.SelectedIndex = 0;
        m_devices = m_layouts.Devices( );
        log.DebugFormat( "Layout - Form Loading - Mappable devices found in templates" );
        foreach ( var dev in m_devices ) {
          log.DebugFormat( "  + {0} - {1}", dev.DevName, dev.DevGuid );
        }
      }
      else {
        log.WarnFormat( "Layout - Form Loading - no layout files found " );
        MessageBox.Show( this, "Could not find any layout templates !??" );
      }

      // Draw Panel
      // drawPanel.Controls.Add( m_dPanel );
      // m_dPanel.Top = 0; m_dPanel.Left = 0;
      log.DebugFormat( "Layout - Form Loading - Add Draw Panel" );
      this.Controls.Add( m_dPanel );
      m_dPanel.Top = 0; m_dPanel.Left = this.Width + 2000; // out of view
      m_dPanel.Visible = false;
      m_dPanel.AutoSize = true;
      m_dPanel.BackgroundImageLayout = ImageLayout.None;
      m_dPanel.BackColor = Color.SpringGreen;
      if ( cbxLayouts.SelectedItem is DeviceLayout ) {
        m_dPanel.BackgroundImageResized = ( cbxLayouts.SelectedItem as DeviceLayout ).Image;
      }

      m_dPanel.Paint += M_dPanel_Paint;

      drawPanel.Controls.Add( m_sPanel );
      m_sPanel.Top = 0; m_sPanel.Left = 0;
      m_sPanel.AutoSize = false;
      m_sPanel.Dock = DockStyle.Fill;
      m_sPanel.BackgroundImageLayout = ImageLayout.Zoom;

      LoadJsCombos( );
      SetJsCombos( ActionList.JsDevices );
      CheckAllGroups( );  // Default is all ON now 

      log.DebugFormat( "Layout - Form Loading - End - Resfresh Panel and show" );
      RefreshPanel( );
    }

    private void FormLayout_FormClosing( object sender, FormClosingEventArgs e )
    {
      log.DebugFormat( "Layout - Form Closing" );
      // don't record minimized, maximized forms
      if ( this.WindowState == FormWindowState.Normal ) {
        AppSettings.Instance.FormLayoutSize = this.Size;
        AppSettings.Instance.FormLayoutLocation = this.Location;
      }
      AppSettings.Instance.Save( );
    }

    #endregion

    #region dPanel Events

    private void M_dPanel_Paint( object sender, PaintEventArgs e )
    {
      Graphics g = e.Graphics;
      m_displayList.DrawList( g );
    }


    private void RefreshPanel()
    {
      m_dPanel.Refresh( );
      var b = new Bitmap( m_dPanel.Width, m_dPanel.Height );
      m_dPanel.DrawToBitmap( b, new Rectangle( 0, 0, b.Width, b.Height ) );
      m_sPanel.BackgroundImage = b;
    }

    #endregion

    /// <summary>
    /// Matches for any selection possible 
    ///  e.g. action maps selected etc.
    /// </summary>
    /// <param name="action">The actionItem</param>
    /// <returns>True to map</returns>
    private bool MatchCriteria( ActionItem action )
    {
      // is the map checked?
      foreach ( int idx in chkLbActionMaps.CheckedIndices ) {
        if ( m_chkLbActionMaps[idx].ActionMap == action.ActionMap ) return true;
      }
      return false; // TODO (add criterias)
    }


    /// <summary>
    /// Populate the display list from the Items found in the ActionTree
    /// </summary>
    private void Populate()
    {
      log.DebugFormat( "Layout - Populate()" );

      bool errorShown = false;

      // for all actions found from action tree
      m_displayList.Clear( );
      if ( !( cbxLayouts.SelectedItem is DeviceLayout ) ) {
        log.WarnFormat( "Layout - Populate() - invalid selected layout.." );
        return;
      }
      log.DebugFormat( "Layout - Populate - Map: {0}", ( cbxLayouts.SelectedItem as DeviceLayout ).DeviceController.MapName );

      ( cbxLayouts.SelectedItem as DeviceLayout ).DeviceController.CreateShapes( );
      foreach ( var actItem in ActionList ) {
        // matches the selected device      
        if ( MatchCriteria( actItem ) ) {
          bool firstInstance = ActionList.IsFirstInstance( actItem.DevicePidVid, actItem.InputTypeNumber );
          var ctrl = ( cbxLayouts.SelectedItem as DeviceLayout ).DeviceController.FindItem( actItem.DevicePidVid, actItem.MainControl, firstInstance );
          if ( ctrl != null ) {
            if ( ctrl.ShapeItems.Count > 0 ) {
              var shape = ctrl.ShapeItems.Dequeue( );
              shape.DispText = actItem.ModdedDispText;
              shape.TextColor = MapProps.MapForeColor( actItem.ActionMap );
              shape.BackColor = MapProps.MapBackColor( actItem.ActionMap );
              if ( shape is ShapeKey ) {
                // kbd map
                ( shape as ShapeKey ).SCGameKey = actItem.MainControl;
              }
              m_displayList.Add( shape );
            }
            else {
              // Display elements exhausted...
              if ( !errorShown ) {
                log.DebugFormat( "Layout - Populate - number of shapes exhausted for device: {0}", actItem.DeviceName );

                string msg = $"No more display elements left for device:  {actItem.DeviceName}";
                msg += $"\n\nTry to use a smaller font to show all actions!";
                MessageBox.Show( msg, "Layout - Cannot show all actions", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                errorShown = true; // only once
              }
            }
          }
        }
      }
      log.DebugFormat( "Layout - Populate() - Done" );
    }

    // Event Handlers

    private void btLayout_Click( object sender, EventArgs e )
    {
      Populate( );
      RefreshPanel( );
    }

    private void cbxLayouts_SelectedIndexChanged( object sender, EventArgs e )
    {
      m_displayList.Clear( );
      m_dPanel.BackgroundImageResized = ( cbxLayouts.SelectedItem as DeviceLayout ).Image;
      log.DebugFormat( "Layout - Selected Layout: {0}", ( cbxLayouts.SelectedItem as DeviceLayout ).Filename );
      RefreshPanel( );
    }

    private void chkLbActionGroups_ItemCheck( object sender, ItemCheckEventArgs e )
    {
      var group = (EGroup)e.Index;
      var gNames = ActionmapNames( group );
      for ( int idx = 0; idx < chkLbActionMaps.Items.Count; idx++ ) {
        if ( gNames.Contains( m_chkLbActionMaps[idx].ActionMap ) ) {
          chkLbActionMaps.Items[idx].Checked = ( e.NewValue == CheckState.Checked ) ? true : false;
        }
      }
    }

    private void btSave_Click( object sender, EventArgs e )
    {
      SFD.Filter = "PNG Files|*.png|Jpg Files|*.jpg|All Files|*.*";
      SFD.DefaultExt = "png";
      if ( SFD.ShowDialog( this ) == DialogResult.OK ) {
        var b = new Bitmap( m_dPanel.Width, m_dPanel.Height );
        m_dPanel.DrawToBitmap( b, new Rectangle( 0, 0, b.Width, b.Height ) );
        string ext = Path.GetExtension( SFD.FileName );
        if ( ext.ToLowerInvariant( ) == ".jpg" ) {
          b.Save( SFD.FileName, ImageFormat.Jpeg );
        }
        else if ( ext.ToLowerInvariant( ) == ".png" ) {
          b.Save( SFD.FileName, ImageFormat.Png );
        }
        else {
          log.DebugFormat( "Layout - SaveFile - unknown file format: {0}", ext.ToLowerInvariant( ) );
          MessageBox.Show( "Unkown fileformat - use jpg or png please", "Save Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
        }

      }

    }

    #region Color Setting Handling

    private int m_colSelGroup = -1; // index of selected group

    private void btColors_Click( object sender, EventArgs e )
    {
      pnlInput.Enabled = false;
      m_sPanel.Visible = false;
      tbFontSize.Value = MapProps.FontSize; lblFontSize.Text = MapProps.FontSize.ToString( );
      lblTest.Font = MapProps.MapFont;
      gbxColors.Visible = true;
    }


    private void btCloseColors_Click( object sender, EventArgs e )
    {
      gbxColors.Visible = false;
      pnlInput.Enabled = true;
      m_sPanel.Visible = true;
    }

    private void btAcceptColors_Click( object sender, EventArgs e )
    {
      // carry values to MapProps
      MapProps.FontSize = int.Parse( lblFontSize.Text );

      foreach ( ListViewItem lv in chkLbActionGroupsColor.Items ) {
        MapProps.SetMapColor( (EGroup)lv.Index, lv.ForeColor, lv.BackColor );
      }
      MapProps.SaveToSettings( );

      // recolor the selection in main
      foreach ( ListViewItem item in chkLbActionGroups.Items ) {
        item.ForeColor = MapProps.GroupColor( (EGroup)item.Index ).ForeColor;
        item.BackColor = MapProps.GroupColor( (EGroup)item.Index ).BackColor;
      }

      foreach ( ListViewItem item in chkLbActionMaps.Items ) {
        item.ForeColor = MapProps.GroupColor( MapNameToGroup( item.ToolTipText ) ).ForeColor;
        item.BackColor = MapProps.GroupColor( MapNameToGroup( item.ToolTipText ) ).BackColor;
      }

      gbxColors.Visible = false;
      pnlInput.Enabled = true;
      m_sPanel.Visible = true;
    }


    private void lblTextColor_Click( object sender, EventArgs e )
    {
      if ( m_colSelGroup >= 0 ) {
        colDlg.Color = lblTest.ForeColor;
        if ( colDlg.ShowDialog( this ) == DialogResult.OK ) {
          lblTest.ForeColor = colDlg.Color;
          chkLbActionGroupsColor.Items[m_colSelGroup].ForeColor = colDlg.Color;
        }
      }
      else {
        MySounds.PlayCannot( );
      }
    }

    private void lblBackColor_Click( object sender, EventArgs e )
    {
      if ( m_colSelGroup >= 0 ) {
        colDlg.Color = lblTest.BackColor;
        if ( colDlg.ShowDialog( this ) == DialogResult.OK ) {
          lblTest.BackColor = colDlg.Color;
          chkLbActionGroupsColor.Items[m_colSelGroup].BackColor = colDlg.Color;
        }
      }
      else {
        MySounds.PlayCannot( );
      }
    }

    private void chkLbActionGroupsColor_ItemActivate( object sender, EventArgs e )
    {
      foreach ( ListViewItem item in chkLbActionGroupsColor.SelectedItems ) {
        m_colSelGroup = item.Index;
        lblTest.Text = item.Text;
        lblTest.ForeColor = item.ForeColor; lblTextColor.BackColor = item.ForeColor;
        lblTest.BackColor = item.BackColor; lblBackColor.BackColor = item.BackColor;
      }
    }

    private void tbFontSize_Scroll( object sender, EventArgs e )
    {
      lblFontSize.Text = tbFontSize.Value.ToString( );
      lblTest.Font = new Font( lblTest.Font.FontFamily, tbFontSize.Value ); ;
    }

    private void pictureBox1_DoubleClick( object sender, EventArgs e )
    {
      CheckAllGroups( );
    }

    private void btClose_Click( object sender, EventArgs e )
    {
      this.Close( );
    }

    #endregion

    #region DEBUG LIST

    private DbgActionItemList DBG_LIST = null;

    private void btActivateDevices_Click( object sender, EventArgs e )
    {
      var guids = new List<string>( );
      if ( !string.IsNullOrEmpty( ( cbxJs1.SelectedItem as Device.DeviceDescriptor ).DevGuid ) ) guids.Add( ( cbxJs1.SelectedItem as Device.DeviceDescriptor ).DevGuid );
      if ( !string.IsNullOrEmpty( ( cbxJs2.SelectedItem as Device.DeviceDescriptor ).DevGuid ) ) guids.Add( ( cbxJs2.SelectedItem as Device.DeviceDescriptor ).DevGuid );
      if ( !string.IsNullOrEmpty( ( cbxJs3.SelectedItem as Device.DeviceDescriptor ).DevGuid ) ) guids.Add( ( cbxJs3.SelectedItem as Device.DeviceDescriptor ).DevGuid );
      if ( !string.IsNullOrEmpty( ( cbxJs4.SelectedItem as Device.DeviceDescriptor ).DevGuid ) ) guids.Add( ( cbxJs4.SelectedItem as Device.DeviceDescriptor ).DevGuid );
      ActionList.OverrideJsDevices( guids ); // inject in current Action Items
    }

    private void btCreateDbgList_Click( object sender, EventArgs e )
    {
      DBG_LIST = new DbgActionItemList( );
      var guids = new List<string>( );
      if ( !string.IsNullOrEmpty( ( cbxJs1.SelectedItem as Device.DeviceDescriptor ).DevGuid ) ) guids.Add( ( cbxJs1.SelectedItem as Device.DeviceDescriptor ).DevGuid );
      if ( !string.IsNullOrEmpty( ( cbxJs2.SelectedItem as Device.DeviceDescriptor ).DevGuid ) ) guids.Add( ( cbxJs2.SelectedItem as Device.DeviceDescriptor ).DevGuid );
      if ( !string.IsNullOrEmpty( ( cbxJs3.SelectedItem as Device.DeviceDescriptor ).DevGuid ) ) guids.Add( ( cbxJs3.SelectedItem as Device.DeviceDescriptor ).DevGuid );
      if ( !string.IsNullOrEmpty( ( cbxJs4.SelectedItem as Device.DeviceDescriptor ).DevGuid ) ) guids.Add( ( cbxJs4.SelectedItem as Device.DeviceDescriptor ).DevGuid );

      DBG_LIST.CreateDebugList( guids.ToArray( ) );
      ActionList = DBG_LIST.DbgList; // create an artificial ActionItems List
    }

    #endregion

  }
}

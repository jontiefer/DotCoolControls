/* Copyright © 2016 Jonathan Tiefer - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the GNU Lesser General Public License (LGPL)
 *
 * You should have received a copy of the LGPL license with
 * this file.
 *
 * /

/*  This file is part of DotCoolControls
*
*   DotCoolControls is free software: you can redistribute it and/or modify
*   it under the terms of the GNU Lesser General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*    (at your option) any later version.
*
*   DotCoolControls is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU Lesser General Public License for more details.
*
*  You should have received a copy of the GNU Lesser General Public License
*   along with DotCoolControls.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tiferix.Global;
using DotCoolControls.Tools;
using DotCoolControls.VisualSettings;

namespace DotCoolControls
{
    #region Control Setting Base Classes

    /// <summary>
    /// A list of a set of all control visual settings that are linked to a DotCoolControl.  The DotCoolCtlSettingsList will be used in a DotCoolControl 
    /// class to give the user access to the set of state-specific visual setting properties that can be organized by the various control states.  Accessing 
    /// the visual setting properties through the ControlSettingsList classes will allow the visual settings to be organized by the various control states 
    /// for easier management of the various visual settings that can be associated with the control.
    /// </summary>
    /// <typeparam name="TSetting">Type of visual setting associated with the control setting list.</typeparam>
    public class DotCoolCtlSettingsList<TSetting>
    {
        #region Events

        /// <summary>
        /// The SettingChanged event will be raised any time a setting in the class is modified.
        /// </summary>
        public event Action<VisualSettingEnum, string, object> SettingChanged;

        #endregion

        #region Member Variables

        private bool m_blRefreshOnChange = false;

        #endregion

        #region Member Object Variables

        /// <summary>
        /// Contains a reference to the DotCoolControl associated with the visual settings accessed through the DotCoolCtlSettings class.
        /// </summary>
        protected Control m_ctlDotCool = null;

        #endregion

        #region Visual Setting Variables

        /// <summary>
        /// Contains the various settings of the DotCoolControl class.
        /// </summary>
        protected VisualSettingProperties<TSetting> m_VisSettings = null;

        #endregion

        #region Control Visual Setting Variables

        /// <summary>
        /// Contains the set of properties that will access and modify the visual settings associated with the DotCoolControl class that are linked to the 
        /// DotCoolControls list.
        /// </summary>
        protected Dictionary<VisualSettingEnum, DotCoolCtlSettings<TSetting>> m_CtlSettings = null;

        #endregion

        #region Constructor/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="control"></param>
        /// <param name="VisSettings"></param>
        public DotCoolCtlSettingsList(Control control, VisualSettingProperties<TSetting> VisSettings = null)
        {
            try
            {
                m_ctlDotCool = control;

                m_CtlSettings = new Dictionary<VisualSettingEnum, DotCoolCtlSettings<TSetting>>();

                if (VisSettings != null)
                    m_VisSettings = VisSettings;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlSettingsList class.");
            }
        }

        #endregion

        #region General Properties

        /// <summary>
        /// Indicates if the linked DotCool control will be refreshed any time a setting is changed in the class.
        /// </summary>
        [Browsable(false)]
        public virtual bool RefreshOnChange
        {
            get
            {
                return m_blRefreshOnChange;
            }
            set
            {
                m_blRefreshOnChange = value;
            }
        }

        #endregion

        #region Visual Setting Access/Selection Properties, Functions

        /// <summary>
        /// Gets the visual setting properties of the type of visual setting linked to the class for the state/scenario specified in the function's setting
        /// parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public virtual TSetting GetVisualSetting(VisualSettingEnum setting)
        {
            try
            {
                return m_VisSettings[setting];
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetVisualSetting function of DotCoolCtlSettingsList class.");
                return default(TSetting);
            }
        }

        #endregion

        #region Control Setting Access/Selection Properties, Functions

        /// <summary>
        /// Gets the control setting properties of the type of setting linked to the class for the state/scenario specified in the function's setting
        /// parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public virtual DotCoolCtlSettings<TSetting> GetControlSetting(VisualSettingEnum setting)
        {
            try
            {
                return m_CtlSettings[setting];
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in GetControlSetting function of DotCoolCtlSettingsList class.");
                return null;
            }
        }

        /* NOT USED: The default property will be implemented for all control setting list classes inherited from the base DotCoolCtlSettingsList class, 
         * so strong typing can be used.
        /// <summary>
        /// Gets the visual setting properties of the type of visual setting linked to the class for the state/scenario specified in the function's setting
        /// parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>        
        public virtual DotCoolCtlSettings<TSetting> this[VisualSettingEnum setting]
        {
            get
            {
                return GetControlSetting(setting);
            }
        }
        */

        #endregion

        #region Event Raising Functions

        /// <summary>
        /// Raises the SettingChanged event.
        /// </summary>
        /// <param name="settingIndex"></param>
        /// <param name="strSettingName"></param>
        /// <param name="value"></param>
        internal virtual void OnSettingChanged(VisualSettingEnum settingIndex, string strSettingName, object value)
        {
            try
            {
                SettingChanged?.Invoke(settingIndex, strSettingName, value);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnSettingChanged function of DotCoolCtlSettingsList class.");
            }
        }

        #endregion
    }

    /// <summary>
    /// Contains the control visual setting propeties for a specific control state associated with a DotCoolControl.  Each control visual setting property set 
    /// will be contained in a DotCoolCtlSettingsList class where it can be accessed through its associated control state.
    /// </summary>
    /// <typeparam name="TSetting">Type of visual setting associated with the control settings list the control setting is contained.</typeparam>
    public abstract class DotCoolCtlSettings<TSetting>
    {
        #region Member Variables
        #endregion

        #region Member Object Variables

        protected DotCoolCtlSettingsList<TSetting> m_CtlSettingsList = null;

        /// <summary>
        /// Contains a reference to the DotCoolControl associated with the specific visual settings class.
        /// </summary>
        protected Control m_ctlDotCool = null;

        #endregion

        #region Visual Setting Access/Selection Properties, Functions

        private VisualSettingEnum m_SettingType = VisualSettingEnum.Normal;

        #endregion

        #region Visual Setting Access/Selection Properties, Functions

        /// <summary>
        /// Indicates which type of visual setting will be associated with the VisualSettingProperties data object linked to the class.  
        /// A setting will be stored for each state/scenario that can be used for any DotCoolControl.  All properties associated with the visual
        /// settings item will be linked to this specified state/scenario.
        /// </summary>
        [Browsable(false)]
        public virtual VisualSettingEnum SettingType
        {
            get
            {
                return m_SettingType;
            }
        }

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CtlSettingsList"></param>
        /// <param name="setting"></param>
        /// <param name="control"></param>
        public DotCoolCtlSettings(DotCoolCtlSettingsList<TSetting> CtlSettingsList, VisualSettingEnum setting, Control control)
        {
            try
            {
                m_CtlSettingsList = CtlSettingsList;
                m_SettingType = setting;
                m_ctlDotCool = control;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlSettings class.");
            }
        }

        #endregion   

        #region General Properties

        /// <summary>
        /// Indicates if the linked DotCool control will be refreshed any time a setting is changed in the class.
        /// </summary>
        [Browsable(false)]
        public virtual bool RefreshOnChange
        {
            get
            {
                return m_CtlSettingsList.RefreshOnChange;
            }
        }

        #endregion

        #region Event Raising Functions

        /// <summary>
        /// Raises the SettingChanged event of the DotCoolCtlSettingsList class.
        /// </summary>
        /// <param name="settingIndex"></param>
        /// <param name="strSettingName"></param>
        /// <param name="value"></param>
        protected virtual void OnSettingChanged(VisualSettingEnum settingIndex, string strSettingName, object value)
        {
            try
            {
                m_CtlSettingsList.OnSettingChanged(settingIndex, strSettingName, value);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnSettingChanged function of DotCoolCtlSettings class.");
            }
        }

        #endregion
    }

    #endregion

    #region Gradient Control Setting Classes

    /// <summary>
    /// A list of a set of all gradient related control visual settings that are linked to a DotCoolControl.  The DotCoolCtlGradientSettingsList will be used
    /// in a DotCoolControl class to give the user access to the set of state-specific gradient visual setting properties that can be organized by the 
    /// various control states.  Accessing the gradient visual setting properties through the GradientSettingsList classes will allow the gradient visual
    /// settings to be organized by the various control states for easier management of the various gradient visual settings that can be associated with the 
    /// control.
    /// </summary>
    public class DotCoolCtlGradientSettingsList : DotCoolCtlSettingsList<GradientVisualSettings>
    {
        #region Member Variables
        #endregion

        #region Member Object Variables        

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="control"></param>
        /// <param name="blRefreshOnChange"></param>
        /// <param name="VisSettings"></param>
        public DotCoolCtlGradientSettingsList(Control control, bool blRefreshOnChange = true, VisualSettingProperties<GradientVisualSettings> VisSettings = null)
            : base(control, VisSettings)
        {
            try
            {
                RefreshOnChange = blRefreshOnChange;

                if(VisSettings == null)
                    m_VisSettings = VisualSettingPropGenerator.CreateGradientVisualSettings();

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    m_CtlSettings.Add(setting, new DotCoolCtlGradientSettings(this, setting, control));
                }//next setting
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlGradientSettingsList class.");
            }
        }

        #endregion

        #region Control Setting Access/Selection Properties, Functions        

        /// <summary>
        /// Gets the visual setting properties of the type of visual setting linked to the class for the state/scenario specified in the function's setting
        /// parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>        
        public virtual DotCoolCtlGradientSettings this[VisualSettingEnum setting]
        {
            get
            {
                return (DotCoolCtlGradientSettings)GetControlSetting(setting);
            }
        }

        #endregion
    }

    /// <summary>
    /// Contains the control gradient visual setting propeties for a specific control state associated with a DotCoolControl.  Each control gradient visual 
    /// setting property set will be contained in a DotCoolCtlGradientSettingsList class where it can be accessed through its associated control state.
    /// </summary>
    public class DotCoolCtlGradientSettings : DotCoolCtlSettings<GradientVisualSettings>
    {
        #region Member Variables
        #endregion

        #region Member Object Variables

        protected DotCoolCtlGradientSettingsList m_GradSettingsList = null;

        #endregion

        #region Visual Setting Access/Selection Properties, Functions

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="GradSettingsList"></param>
        /// <param name="setting"></param>
        /// <param name="control"></param>
        public DotCoolCtlGradientSettings(DotCoolCtlGradientSettingsList GradSettingsList, VisualSettingEnum setting, Control control)
            : base(GradSettingsList, setting, control)
        {
            try
            {
                m_GradSettingsList = GradSettingsList;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlGradientSettings class.");
            }
        }

        #endregion

        #region General Overriden Properties, Functions

        /// <summary>
        /// Overrides the ToString function which will allow an appropriate caption to be displayed in the property grid for the specific type of control 
        /// visual setting.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                return SettingType.ToString() + " Gradient Settings";
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ToString function of DotCoolCtlGradientSettings class.");
                return "Error";
            }
        }

        #endregion

        #region Gradient Color and Gradient Setting Properties, Functions

        /// <summary>
        /// The starting color in the gradient path to be used for drawing the gradient image of the control.
        /// </summary>        
        [Browsable(true), Category("CoolGradient"),
         Description("The starting color in the gradient path to be used for drawing the gradient image of the " +
                          "control.")]
        public virtual Color GradientColor1
        {
            get
            {
                return m_GradSettingsList.GetVisualSetting(SettingType).GradientColor1;
            }
            set
            {
                m_GradSettingsList.GetVisualSetting(SettingType).GradientColor1 = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "Gradient1", value);
            }
        }

        /// <summary>
        /// The ending color in the gradient path to be used for drawing the gradient image of the control.
        /// </summary>
        [Browsable(true), Category("CoolGradient"),
          Description("The ending color in the gradient path to be used for drawing the gradient background of the control.")]
        public virtual Color GradientColor2
        {
            get
            {
                return m_GradSettingsList.GetVisualSetting(SettingType).GradientColor2;
            }
            set
            {
                m_GradSettingsList.GetVisualSetting(SettingType).GradientColor2 = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "GradientColor2", value);
            }
        }

        /// <summary>
        /// Indicates the style/pattern to be used for drawing the DotCoolControl's gradient image.
        /// </summary>
        [Browsable(true), Category("CoolGradient"), DefaultValue(CoolGradientType.None),
         Description("Indicates the style/pattern to be used for drawing the DotCoolControl's gradient image.")]
        public virtual CoolGradientType GradientType
        {
            get
            {
                return m_GradSettingsList.GetVisualSetting(SettingType).GradientType;
            }
            set
            {
                m_GradSettingsList.GetVisualSetting(SettingType).GradientType = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "GradientType", value);
            }
        }

        /// <summary>
        /// The span (size factor or expanse) of the gradient image to be used for drawing the gradient image of the DotCoolControl control. 
        /// Certain types of gradients will look more appealing in a control when drawn on a larger or smaller expanse.  Most gradients will look  
        /// ideal drawn with the default gradient span associated with the gradient type.
        /// </summary>        
        [Browsable(true), Category("CoolGradient"), DefaultValue(1f),
         Description("The span (size factor or expanse) of the gradient image to be used for drawing the gradient image of the DotCoolControl control.")]
        public virtual float GradientSpan
        {
            get
            {
                return m_GradSettingsList.GetVisualSetting(SettingType).GradientSpan;
            }
            set
            {
                m_GradSettingsList.GetVisualSetting(SettingType).GradientSpan = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "GradientSpan", value);
            }
        }

        /// <summary>
        /// Indicates if the default gradient span (size factor or expanse) associated with the selected gradient type used for drawing the gradient
        /// image will be used for the control.  Every gradient type has a default gradient span setting that usually will look most ideal for
        /// the type of gradient being drawn.  When this flag is set to true, the gradient span setting cannot be modified in the control, as the default 
        /// value will be used.
        /// </summary>        
        [Browsable(true), Category("CoolGradient"), DefaultValue(true),
         Description("Indicates if the default gradient span (size factor or expanse) associated with the selected gradient type used for drawing the gradient " +
                           "image will be used for the control.")]
        public virtual bool UseDefaultGradientSpan
        {
            get
            {
                return m_GradSettingsList.GetVisualSetting(SettingType).UseDefaultGradientSpan;
            }
            set
            {
                m_GradSettingsList.GetVisualSetting(SettingType).UseDefaultGradientSpan = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "UseDefaultGradientSpan", value);
            }
        }

        /// <summary>
        /// The X and Y offset position of the gradient image to be used for drawing the gradient background of the DotCoolControl control.  Adjusting 
        /// the offset position of the gradient alters the lighting and color range of the gradient image in the control.
        /// </summary>        
        [Browsable(true), Category("CoolGradient"),
         Description("The X and Y offset position of the gradient image to be used for drawing the gradient background of the DotCoolControl control.")]
        public virtual Point GradientOffset
        {
            get
            {
                return m_GradSettingsList.GetVisualSetting(SettingType).GradientOffset;
            }
            set
            {
                m_GradSettingsList.GetVisualSetting(SettingType).GradientOffset = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "GradientOffset", value);
            }
        }

        #endregion        
    }

    #endregion

    #region Text Control Setting Classes

    /// <summary>
    /// A list of a set of all text/caption related control visual settings that are linked to a DotCoolControl.  The DotCoolCtlTextSettingsList will be used
    /// in a DotCoolControl class to give the user access to the set of state-specific text/caption visual setting properties that can be organized by the 
    /// various control states.  Accessing the text/caption visual setting properties through the TextSettingsList classes will allow the text visual
    /// settings to be organized by the various control states for easier management of the various text visual settings that can be associated with the 
    /// control.
    /// </summary>
    public class DotCoolCtlTextSettingsList : DotCoolCtlSettingsList<TextVisualSettings>
    {
        #region Member Variables
        #endregion

        #region Member Object Variables        

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="control"></param>
        /// <param name="blRefreshOnChange"></param>
        /// <param name="VisSettings"></param>
        public DotCoolCtlTextSettingsList(Control control, bool blRefreshOnChange = true, VisualSettingProperties<TextVisualSettings> VisSettings = null)
            : base(control, VisSettings)
        {
            try
            {
                RefreshOnChange = blRefreshOnChange;

                if(VisSettings == null)
                    m_VisSettings = VisualSettingPropGenerator.CreateTextVisualSettings();

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    m_CtlSettings.Add(setting, new DotCoolCtlTextSettings(this, setting, control));
                }//next setting
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlTextSettingsList class.");
            }
        }

        #endregion

        #region Control Setting Access/Selection Properties, Functions        

        /// <summary>
        /// Gets the visual setting properties of the type of visual setting linked to the class for the state/scenario specified in the function's setting
        /// parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>        
        public virtual DotCoolCtlTextSettings this[VisualSettingEnum setting]
        {
            get
            {
                return (DotCoolCtlTextSettings)GetControlSetting(setting);
            }
        }

        #endregion
    }

    /// <summary>
    /// Contains the control text visual setting propeties for a specific control state associated with a DotCoolControl.  Each control text/caption visual 
    /// setting property set will be contained in a DotCoolCtlTextSettingsList class where it can be accessed through its associated control state.
    /// </summary>
    public class DotCoolCtlTextSettings : DotCoolCtlSettings<TextVisualSettings>
    {
        #region Member Variables
        #endregion

        #region Member Object Variables

        protected DotCoolCtlTextSettingsList m_TextSettingsList = null;

        #endregion

        #region Visual Setting Access/Selection Properties, Functions

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TextSettingsList"></param>
        /// <param name="setting"></param>
        /// <param name="control"></param>
        public DotCoolCtlTextSettings(DotCoolCtlTextSettingsList TextSettingsList, VisualSettingEnum setting, Control control)
            : base(TextSettingsList, setting, control)
        {
            try
            {
                m_TextSettingsList = TextSettingsList;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlTextSettings class.");
            }
        }

        #endregion

        #region General Overriden Properties, Functions

        /// <summary>
        /// Overrides the ToString function which will allow an appropriate caption to be displayed in the property grid for the specific type of control 
        /// visual setting.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                return SettingType.ToString() + " Text Settings";
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ToString function of DotCoolCtlTextSettings class.");
                return "Error";
            }
        }

        #endregion

        #region Text/Caption Properties, Functions

        /// <summary>
        /// Indicates if the text settings of the DotCoolControl will be enabled for the associated control state.
        /// </summary>
        [Browsable(true), Category("CoolText"),
         Description("Indicates if the text settings of the DotCoolControl will be enabled for the associated control state.")]
        public virtual bool EnableText
        {
            get
            {
                if (SettingType == VisualSettingEnum.Normal)
                    return true;
                else
                    return m_TextSettingsList.GetVisualSetting(SettingType).EnableText;
            }
            set
            {
                try
                {
                    if (SettingType == VisualSettingEnum.Normal && !value)
                    {
                        m_TextSettingsList.GetVisualSetting(SettingType).EnableText = true;
                        
                        MessageBox.Show("The text settings for a normal state will always be enabled and cannot be modified.",
                                                    "Cannot Modify Normal State", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        return;
                    }
                    else
                    {
                        m_TextSettingsList.GetVisualSetting(SettingType).EnableText = value;

                        if (RefreshOnChange)
                            m_ctlDotCool.Refresh();

                        OnSettingChanged(SettingType, "Font", value);
                    }//end if
                }
                catch (Exception err)
                {
                    ErrorHandler.ShowErrorMessage(err, "Error in EnableText Set property of DotCoolCtlTextSettings class.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the font of the text displayed in the control.
        /// </summary>
        [Browsable(true), Category("CoolText"),
         Description("Gets or sets the font of the text displayed in the DotCoolControl..")]
        public Font Font
        {
            get
            {
                return m_TextSettingsList.GetVisualSetting(SettingType).Font;
            }
            set
            {
                m_TextSettingsList.GetVisualSetting(SettingType).Font = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "Font", value);
            }
        }

        /// <summary>
        ///  Gets or sets the foreground color of the control.
        /// </summary>
        [Browsable(true), Category("CoolText"),
         Description("Gets or sets the foreground color of the DotCoolControl..")]
        public Color ForeColor
        {
            get
            {
                return m_TextSettingsList.GetVisualSetting(SettingType).ForeColor;
            }
            set
            {
                m_TextSettingsList.GetVisualSetting(SettingType).ForeColor = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "ForeColor", value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the caption in the control.
        /// </summary>
        [Browsable(true), Category("CoolText"), DefaultValue(ContentAlignment.TopLeft),
         Description("Gets or sets the alignment of the caption in the control.")]
        public virtual ContentAlignment TextAlign
        {
            get
            {
                return m_TextSettingsList.GetVisualSetting(SettingType).TextAlign;
            }
            set
            {
                m_TextSettingsList.GetVisualSetting(SettingType).TextAlign = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "TextAlign", value);
            }
        }

        /// <summary>
        /// Gets or sets the x and y offsets of the caption in the control.
        /// </summary>
        [Browsable(true), Category("CoolText"),
         Description("Gets or sets the x and y offsets of the caption in the control.")]
        public virtual Point TextOffset
        {
            get
            {
                return m_TextSettingsList.GetVisualSetting(SettingType).TextOffset;
            }
            set
            {
                m_TextSettingsList.GetVisualSetting(SettingType).TextOffset = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "TextOffset", value);
            }
        }

        #endregion
    }

    #endregion

    #region Image Setting Classes

    /// <summary>
    /// A list of a set of all image related control visual settings that are linked to a DotCoolControl.  The DotCoolCtlImageSettingsList will be used
    /// in a DotCoolControl class to give the user access to the set of state-specific image visual setting properties that can be organized by the 
    /// various control states.  Accessing the image visual setting properties through the ImageSettingsList classes will allow the image visual
    /// settings to be organized by the various control states for easier management of the various image visual settings that can be associated with the 
    /// control.
    /// </summary>
    public class DotCoolCtlImageSettingsList : DotCoolCtlSettingsList<ImageVisualSettings>
    {
        #region Member Variables
        #endregion

        #region Member Object Variables        

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="control"></param>
        /// <param name="blRefreshOnChange"></param>
        /// <param name="VisSettings"></param>
        public DotCoolCtlImageSettingsList(Control control, bool blRefreshOnChange = true, VisualSettingProperties<ImageVisualSettings> VisSettings = null)
            : base(control, VisSettings)
        {
            try
            {
                RefreshOnChange = blRefreshOnChange;

                if(VisSettings == null)
                    m_VisSettings = VisualSettingPropGenerator.CreateImageVisualSettings();

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    m_CtlSettings.Add(setting, new DotCoolCtlImageSettings(this, setting, control));
                }//next setting
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlImageSettingsList class.");
            }
        }

        #endregion

        #region Control Setting Access/Selection Properties, Functions        

        /// <summary>
        /// Gets the visual setting properties of the type of visual setting linked to the class for the state/scenario specified in the function's setting
        /// parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>        
        public virtual DotCoolCtlImageSettings this[VisualSettingEnum setting]
        {
            get
            {
                return (DotCoolCtlImageSettings)GetControlSetting(setting);
            }
        }

        #endregion
    }

    /// <summary>
    /// Contains the control image visual setting propeties for a specific control state associated with a DotCoolControl.  Each control image visual 
    /// setting property set will be contained in a DotCoolCtlImageSettingsList class where it can be accessed through its associated control state.
    /// </summary>
    public class DotCoolCtlImageSettings : DotCoolCtlSettings<ImageVisualSettings>
    {
        #region Member Variables
        #endregion

        #region Member Object Variables

        protected DotCoolCtlImageSettingsList m_ImageSettingsList = null;

        #endregion

        #region Visual Setting Access/Selection Properties, Functions

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ImageSettingsList"></param>
        /// <param name="setting"></param>
        /// <param name="control"></param>
        public DotCoolCtlImageSettings(DotCoolCtlImageSettingsList ImageSettingsList, VisualSettingEnum setting, Control control)
            : base(ImageSettingsList, setting, control)
        {
            try
            {
                m_ImageSettingsList = ImageSettingsList;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlImageSettings class.");
            }
        }

        #endregion

        #region General Overriden Properties, Functions

        /// <summary>
        /// Overrides the ToString function which will allow an appropriate caption to be displayed in the property grid for the specific type of control 
        /// visual setting.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                return SettingType.ToString() + " Image Settings";
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ToString function of DotCoolCtlImageSettings class.");
                return "Error";
            }
        }

        #endregion

        #region Image Setting Properties, Functions

        /// <summary>
        /// Gets or sets the image displayed in the control.
        /// </summary>
        [Browsable(true), Category("CoolImage"), DefaultValue(null),
         Description("Gets or sets the image displayed in the control.")]
        public virtual Image Image
        {
            get
            {
                return m_ImageSettingsList.GetVisualSetting(SettingType).Image;
            }
            set
            {
                m_ImageSettingsList.GetVisualSetting(SettingType).Image = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "Image", value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the image in the control.
        /// </summary>
        [Browsable(true), Category("CoolImage"), DefaultValue(ContentAlignment.TopLeft),
         Description("Gets or sets the alignment of the image in the control.")]
        public virtual ContentAlignment ImageAlign
        {
            get
            {
                return m_ImageSettingsList.GetVisualSetting(SettingType).ImageAlign;
            }
            set
            {
                m_ImageSettingsList.GetVisualSetting(SettingType).ImageAlign = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "ImageAlign", value);
            }
        }

        /// <summary>
        /// Gets or sets the x and y offset position of the image in the control.
        /// </summary>
        [Browsable(true), Category("CoolImage"),
         Description("Gets or sets the x and y offset position of the image in the control.")]
        public virtual Point ImageOffset
        {
            get
            {
                return m_ImageSettingsList.GetVisualSetting(SettingType).ImageOffset;
            }
            set
            {
                m_ImageSettingsList.GetVisualSetting(SettingType).ImageOffset = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "ImageOffset", value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the image to use for transparency in the control.
        /// </summary>
        [Browsable(true), Category("CoolImage"),
         Description("Gets or sets the color of the image to use for transparency in the control.")]
        public virtual Color ImageTransColor
        {
            get
            {
                return m_ImageSettingsList.GetVisualSetting(SettingType).TransparentColor;
            }
            set
            {
                m_ImageSettingsList.GetVisualSetting(SettingType).TransparentColor = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "ImageTransColor", value);
            }
        }

        /// <summary>
        /// Indicates if the image settings will be used for the associated control state.
        /// </summary>
        [Browsable(true), Category("CoolImage"),
         Description("Indicates if the image settings will be used for the associated control state.")]
        public virtual bool EnableImage
        {
            get
            {
                if (SettingType == VisualSettingEnum.Normal)
                    return true;
                else
                    return m_ImageSettingsList.GetVisualSetting(SettingType).EnableImage;
            }
            set
            {                
                if (SettingType == VisualSettingEnum.Normal && !value)
                {
                    m_ImageSettingsList.GetVisualSetting(SettingType).EnableImage = true;

                    MessageBox.Show("The image settings for a normal state will always be enabled and cannot be modified.",
                                                "Cannot Modify Normal State", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                    
                }
                else
                {
                    m_ImageSettingsList.GetVisualSetting(SettingType).EnableImage = value;

                    if (RefreshOnChange)
                        m_ctlDotCool.Refresh();

                    OnSettingChanged(SettingType, "EnableImage", value);
                }//end if
            }
        }

        #endregion
    }

    #endregion

    #region Border Setting Classes

    /// <summary>
    /// A list of a set of all border related control visual settings that are linked to a DotCoolControl.  The DotCoolCtlBorderSettingsList will be used
    /// in a DotCoolControl class to give the user access to the set of state-specific border visual setting properties that can be organized by the 
    /// various control states.  Accessing the border visual setting properties through the BorderSettingsList classes will allow the border visual
    /// settings to be organized by the various control states for easier management of the various border visual settings that can be associated with the 
    /// control.
    /// </summary>
    public class DotCoolCtlBorderSettingsList : DotCoolCtlSettingsList<BorderVisualSettings>
    {
        #region Member Variables
        #endregion

        #region Member Object Variables        

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="control"></param>
        /// <param name="blRefreshOnChange"></param>
        /// <param name="VisSettings"></param>
        public DotCoolCtlBorderSettingsList(Control control, bool blRefreshOnChange = true, VisualSettingProperties<BorderVisualSettings> VisSettings = null)
            : base(control, VisSettings)
        {
            try
            {
                RefreshOnChange = blRefreshOnChange;

                if(VisSettings == null)
                    m_VisSettings = VisualSettingPropGenerator.CreateBorderVisualSettings();

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    m_CtlSettings.Add(setting, new DotCoolCtlBorderSettings(this, setting, control));
                }//next setting
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlBorderSettingsList class.");
            }
        }

        #endregion

        #region Control Setting Access/Selection Properties, Functions        

        /// <summary>
        /// Gets the visual setting properties of the type of visual setting linked to the class for the state/scenario specified in the function's setting
        /// parameter.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>        
        public virtual DotCoolCtlBorderSettings this[VisualSettingEnum setting]
        {
            get
            {
                return (DotCoolCtlBorderSettings)GetControlSetting(setting);
            }
        }

        #endregion
    }

    /// <summary>
    /// Contains the control border visual setting propeties for a specific control state associated with a DotCoolControl.  Each control border visual 
    /// setting property set will be contained in a DotCoolCtlBorderSettingsList class where it can be accessed through its associated control state.
    /// </summary>
    public class DotCoolCtlBorderSettings : DotCoolCtlSettings<BorderVisualSettings>
    {
        #region Member Variables
        #endregion

        #region Member Object Variables
        
        protected DotCoolCtlBorderSettingsList m_BorderSettingsList = null;

        #endregion

        #region Visual Setting Access/Selection Properties, Functions

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="BorderSettingsList"></param>
        /// <param name="setting"></param>
        /// <param name="control"></param>
        public DotCoolCtlBorderSettings(DotCoolCtlBorderSettingsList BorderSettingsList, VisualSettingEnum setting, Control control)
            : base(BorderSettingsList, setting, control)
        {
            try
            {
                m_BorderSettingsList = BorderSettingsList;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlBorderSettings class.");
            }
        }

        #endregion

        #region General Overriden Properties, Functions

        /// <summary>
        /// Overrides the ToString function which will allow an appropriate caption to be displayed in the property grid for the specific type of control 
        /// visual setting.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                return SettingType.ToString() + " Border Settings";
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ToString function of DotCoolCtlBorderSettings class.");
                return "Error";
            }
        }

        #endregion

        #region Border Appearance Setting Properties, Functions

        /// <summary>
        /// Gets or sets the shape of the control.
        /// </summary>
        [Browsable(true), Category("CoolBorder"), DefaultValue(CoolShape.Rectangle),
         Description("Gets or sets the shape of the control.")]
        public virtual CoolShape BorderShape
        {
            get
            {
                return m_BorderSettingsList.GetVisualSetting(SettingType).BorderShape;
            }
            set
            {
                m_BorderSettingsList.GetVisualSetting(SettingType).BorderShape = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "BorderShape", value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the control's border.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the color of the control's border.")]
        public virtual Color BorderColor
        {
            get
            {
                return m_BorderSettingsList.GetVisualSetting(SettingType).BorderColor;
            }
            set
            {
                m_BorderSettingsList.GetVisualSetting(SettingType).BorderColor = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "BorderColor", value);
            }
        }

        /// <summary>
        /// Gets or sets the radius of the border's corners when drawing shapes that have curved corners or edges.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the radius of the border's corners when drawing shapes that have curved corners or edges.")]
        public virtual Point BorderRadius
        {
            get
            {
                return m_BorderSettingsList.GetVisualSetting(SettingType).BorderRadius;
            }
            set
            {
                m_BorderSettingsList.GetVisualSetting(SettingType).BorderRadius = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "BorderShape", value);
            }
        }

        #endregion

        #region Border Size and Position Setting Properties, Functions

        /// <summary>
        /// Gets or sets the size of the box portion of the control.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the size of the box portion of the control.")]
        public virtual Size BorderSize
        {
            get
            {
                return m_BorderSettingsList.GetVisualSetting(SettingType).BorderSize;
            }
            set
            {
                m_BorderSettingsList.GetVisualSetting(SettingType).BorderSize = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "BorderSize", value);
            }
        }

        /// <summary>
        /// Gets or sets the X and Y offset position of the box portion of the control.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the X and Y offset position of the box portion of the control.")]
        public virtual Point BorderOffset
        {
            get
            {
                return m_BorderSettingsList.GetVisualSetting(SettingType).BorderOffset;
            }
            set
            {
                m_BorderSettingsList.GetVisualSetting(SettingType).BorderOffset = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "BorderOffset", value);
            }
        }

        /// <summary>
        /// Gets or sets the width (thickness) of the control's border.
        /// </summary>
        [Browsable(true), Category("CoolBorder"), DefaultValue(1),
         Description("Gets or sets the width (thickness) of the control's border.")]
        public virtual int BorderWidth
        {
            get
            {
                return m_BorderSettingsList.GetVisualSetting(SettingType).BorderWidth;
            }
            set
            {
                m_BorderSettingsList.GetVisualSetting(SettingType).BorderWidth = value;

                if (RefreshOnChange)
                    m_ctlDotCool.Refresh();

                OnSettingChanged(SettingType, "BorderWidth", value);
            }
        }

        #endregion
    }

    #endregion

    #region Specific Border Setting Class

    /// <summary>
    /// Contains the control border color setting propeties for a specific control state associated with a DotCoolControl.  The control border color 
    /// setting property set wil allow user's to access various border color settings from the property designer in an organized manner.  Each 
    /// border color property setting will be linked to a border visual setting contained in a DotCoolCtlBorderSetingsList class.
    /// </summary>
    public class DotCoolCtlBorderColorSettings
    {
        #region Member Variables
        #endregion

        #region Member Object Variables

        protected DotCoolCtlBorderSettingsList m_BorderSettingsList = null;

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="BorderSettingsList"></param>
        public DotCoolCtlBorderColorSettings(DotCoolCtlBorderSettingsList BorderSettingsList)
        {
            try
            {
                m_BorderSettingsList = BorderSettingsList;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolCtlBorderColorSettings class.");
            }
        }

        #endregion

        #region General Overriden Properties, Functions

        /// <summary>
        /// Overrides the ToString function which will allow an appropriate caption to be displayed in the property grid for the specific type of control 
        /// visual setting.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                return "Border Color Settings";
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ToString function of DotCoolCtlBorderColorSettings class.");
                return "Error";
            }
        }

        #endregion

        #region State Specific Border Color Properties, Functions

        /// <summary>
        /// Gets or sets the color of the control's border.
        /// </summary>
        [Browsable(true), Category("CoolBorder"), DefaultValue(0xff000000),
         Description("Gets or sets the color of the control's border.")]
        public virtual Color BorderColor
        {
            get
            {
                return m_BorderSettingsList[VisualSettingEnum.Normal].BorderColor;
            }
            set
            {
                m_BorderSettingsList[VisualSettingEnum.Normal].BorderColor = value;                
            }
        }

        /// <summary>
        /// Gets or sets the color of the control's border when the control is disabled.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the color of the control's border when the control is disabled.")]
        public virtual Color BorderColorDisabled
        {
            get
            {
                return m_BorderSettingsList[VisualSettingEnum.Disabled].BorderColor;
            }
            set
            {
                m_BorderSettingsList[VisualSettingEnum.Disabled].BorderColor = value;                
            }
        }

        /// <summary>
        /// Gets or sets the color of the control's border when the mouse cursor is positioned over the control.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the color of the control's border when the mouse cursor is positioned over the control.")]
        public virtual Color BorderColorMouseOver
        {
            get
            {
                return m_BorderSettingsList[VisualSettingEnum.MouseOver].BorderColor;
            }
            set
            {
                m_BorderSettingsList[VisualSettingEnum.MouseOver].BorderColor = value;                
            }
        }

        /// <summary>
        /// Gets or sets the color of the control's border when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolBorder"),
         Description("Gets or sets the color of the control's border when the mouse button is pushed down on the control.")]
        public virtual Color BorderColorMouseDown
        {
            get
            {
                return m_BorderSettingsList[VisualSettingEnum.MouseDown].BorderColor;
            }
            set
            {
                m_BorderSettingsList[VisualSettingEnum.MouseDown].BorderColor = value;                
            }
        }

        #endregion
    }


    #endregion
}

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
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tiferix.Global;
using DotCoolControls.Tools;

namespace DotCoolControls.VisualSettings
{
    #region Enumerations

    /// <summary>
    /// This enumeration contains the various control states/scenarios that can be associated with each visual setting of a DotCoolControl.
    /// </summary>
    public enum VisualSettingEnum
    {
        Normal = 1,
        Disabled = 2,
        MouseDown = 3,
        MouseOver = 4,
        Indeterminate = 5
    }

    #endregion    

    /// <summary>
    /// The VisualSettingsProperties class is a templated class that is used for accessing various types of VisualSettings class objects.  Each VisualSetting
    /// class will be stored and accessed in the VisualSettingsProperties class for every possible state associated with visual settings in the program.
    /// </summary>
    /// <typeparam name="TSetting"></typeparam>
    public class VisualSettingProperties<TSetting>
    {
        #region Data Object Variables

        /// <summary>
        /// A reference to each visual setting for every control state/scenario associated with a DotCoolControl. 
        /// </summary>
        private Dictionary<VisualSettingEnum, TSetting> m_Settings = new Dictionary<VisualSettingEnum, TSetting>(4);

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public VisualSettingProperties()
        {
            try
            {
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor funciton of VisualSettingProperties class.");
            }
        }

        #endregion

        #region Visual Setting Adding, Loading, and Removing Properties and Functions

        /// <summary>
        /// Gets/Sets the appropriate visual setting stored in the VisualSettingProperties class according to the specified state of the visual setting.  
        /// A set of different settings for each specific type of VisualSetting will be stored in the VisualSettingsProperty class that are associated 
        /// with the various states that the setting can be accessed.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public TSetting this[VisualSettingEnum setting]
        {
            get
            {
                return m_Settings[setting];
            }
            set
            {
                m_Settings[setting] = value;
            }
        }

        /// <summary>
        /// Adds a visual setting to the VisualSettingsProperties class's visual settings collection.  The visual setting will be added and keyed according to
        /// the state associated with the properties of that setting.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="data"></param>
        public void AddSetting(VisualSettingEnum setting, TSetting data)
        {
            try
            {
                m_Settings.Add(setting, data);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in AddSetting function of VisualSettingProperties class.");
            }
        }

        /// <summary>
        /// Adds a group of visual settings to the VisualSettingsProperties class's visual settings collection.  The visual settings will be added and keyed 
        /// according to the state associated with the properties of each setting.
        /// </summary>
        /// <param name="arySettings"></param>        
        public void AddSettings(KeyValuePair<VisualSettingEnum, TSetting>[] arySettings)
        {
            try
            {
                foreach (KeyValuePair<VisualSettingEnum, TSetting> setting in arySettings)
                {
                    AddSetting(setting.Key, setting.Value);
                }//next setting
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in AddSettings function of VisualSettingProperties class.");
            }
        }

        /// <summary>
        /// Clears all visual settings stored in the VisualSettingsProperties class.
        /// </summary>
        public void ClearSettings()
        {
            try
            {
                m_Settings.Clear();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in ClearSettings function of VisualSettingProperties class.");
            }
        }

        #endregion        
    }

    /// <summary>
    /// This class will be used to generate various type of visual setting properties that will be used in various DotCool controls.  The class can also 
    /// initialize each visual setting property with default values.
    /// </summary>
    public static class VisualSettingPropGenerator
    {
        #region Gradient Visual Setting Generation Static Functions

        /// <summary>
        /// Generates a GradientVisualSettings properties class object and initializes each of its associated properties of the gradient visual settings with 
        /// default values.  A different visual setting will be stored for each of the various states each property can be accessed in their associated control. 
        /// </summary>
        /// <param name="DefaultGradColor1"></param>
        /// <param name="DefaultGradColor2"></param>
        /// <param name="DefaultGradType"></param>
        /// <param name="fDefaultGradSpan"></param>
        /// <param name="blUseDefaultGradSpan"></param>
        /// <param name="ptDefaultGradOffset"></param>
        /// <param name="blDrawGradient"></param>
        /// <returns></returns>
        public static VisualSettingProperties<GradientVisualSettings> CreateGradientVisualSettings(
                                                                                                    Color DefaultGradColor1, Color DefaultGradColor2,
                                                                                                    CoolGradientType DefaultGradType, float fDefaultGradSpan, 
                                                                                                    bool blUseDefaultGradSpan, Point ptDefaultGradOffset, bool blDrawGradient)
        {
            try
            {
                VisualSettingProperties<GradientVisualSettings> gradSettingProps = new VisualSettings.VisualSettingProperties<VisualSettings.GradientVisualSettings>();

                for (int i = 0; i <= 5; i++)
                {
                    VisualSettingEnum setting = (VisualSettingEnum)i;
                    GradientVisualSettings gradVisSetting =
                                        new GradientVisualSettings
                                        {
                                            GradientColor1 = DefaultGradColor1,
                                            GradientColor2 = DefaultGradColor2,
                                            GradientType = DefaultGradType,
                                            GradientSpan = fDefaultGradSpan,
                                            UseDefaultGradientSpan = blUseDefaultGradSpan,
                                            GradientOffset = ptDefaultGradOffset,
                                            DrawGradient = blDrawGradient
                                        };

                    gradSettingProps.AddSetting(setting, gradVisSetting);
                }//next i

                return gradSettingProps;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CreateGradientVisualSettings Overload 1 function of VisualSettingPropGenerator class.");
                return null;
            }
        }

        /// <summary>
        /// Generates a GradientVisualSettings properties class object and initializes each of its associated properties of the gradient visual settings with 
        /// default values.  A different visual setting will be stored for each of the various states each property can be accessed in their associated control. 
        /// </summary>
        /// <returns></returns>
        public static VisualSettingProperties<GradientVisualSettings> CreateGradientVisualSettings()
        {
            try
            {
                return CreateGradientVisualSettings(Color.Transparent, Color.Transparent, CoolGradientType.None, 1f, true, new Point(0, 0), true);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CreateGradientVisualSettings Overload 2 function of VisualSettingPropGenerator class.");
                return null;
            }
        }

        #endregion

        #region Border Visual Setting Generation Static Functions

        /// <summary>
        /// Generates a BorderVisualSettings properties class object and initializes each of its associated properties of the border visual settings with 
        /// default values.  A different visual setting will be stored for each of the various states each property can be accessed in their associated control. 
        /// </summary>
        /// <param name="DefaultBorderShape"></param>
        /// <param name="DefaultBorderColor"></param>
        /// <param name="DefaultBorderSize"></param>
        /// <param name="DefaultBorderRadius"></param>
        /// <param name="DefaultBorderOffset"></param>
        /// <param name="DefaultBorderWidth"></param>
        /// <returns></returns>
        public static VisualSettingProperties<BorderVisualSettings> CreateBorderVisualSettings(
                                                                                                    CoolShape DefaultBorderShape, Color DefaultBorderColor,
                                                                                                    Size DefaultBorderSize, Point DefaultBorderRadius, Point DefaultBorderOffset,
                                                                                                    int DefaultBorderWidth)
        {
            try
            {
                VisualSettingProperties<BorderVisualSettings> borderSettingProps = new VisualSettings.VisualSettingProperties<VisualSettings.BorderVisualSettings>();

                for (int i = 0; i <= 5; i++)
                {
                    VisualSettingEnum setting = (VisualSettingEnum)i;
                    BorderVisualSettings borderVisSetting =
                                        new BorderVisualSettings
                                        {
                                            BorderShape = DefaultBorderShape,
                                            BorderColor = DefaultBorderColor,
                                            BorderSize = DefaultBorderSize,
                                            BorderRadius = DefaultBorderRadius,
                                            BorderOffset = DefaultBorderOffset,
                                            BorderWidth = DefaultBorderWidth
                                        };

                    borderSettingProps.AddSetting(setting, borderVisSetting);
                }//next i

                return borderSettingProps;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CreateBorderVisualSettings Overload 1 function of VisualSettingPropGenerator class.");
                return null;
            }
        }

        /// <summary>
        /// Generates a BorderVisualSettings properties class object and initializes each of its associated properties of the border visual settings with 
        /// default values.  A different visual setting will be stored for each of the various states each property can be accessed in their associated control. 
        /// </summary>
        /// <returns></returns>
        public static VisualSettingProperties<BorderVisualSettings> CreateBorderVisualSettings()
        {
            try
            {
                return CreateBorderVisualSettings(CoolShape.Rectangle, Color.Black, new Size(15, 15), new Point(20, 20), new Point(1, 1), 1);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CreateBorderVisualSettings Overload 2 function of VisualSettingPropGenerator class.");
                return null;
            }
        }

        #endregion

        #region Text Visual Setting Generation Static Functions

        /// <summary>
        /// Generates a TextVisualSettings properties class object and initializes each of its associated properties of the text visual settings with 
        /// default values.  A different visual setting will be stored for each of the various states each property can be accessed in their associated control. 
        /// </summary>
        /// <returns></returns>
        public static VisualSettingProperties<TextVisualSettings> CreateTextVisualSettings(
                                                                                            Font DefaultFont, Color DefaultForeColor, ContentAlignment DefaultTextAlign,
                                                                                            Point ptDefaultTextOffset, bool blDefaultEnableText)
        {
            try
            {
                VisualSettingProperties<TextVisualSettings> textSettingProps = new VisualSettings.VisualSettingProperties<VisualSettings.TextVisualSettings>();

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {                    
                    TextVisualSettings textVisSetting =
                                        new TextVisualSettings
                                        {
                                            Font = DefaultFont,
                                            ForeColor = DefaultForeColor,
                                            TextAlign = DefaultTextAlign,
                                            TextOffset = ptDefaultTextOffset,
                                            EnableText = (setting != VisualSettingEnum.Normal) ? blDefaultEnableText : true
                                        };

                    textSettingProps.AddSetting(setting, textVisSetting);
                }//next i

                return textSettingProps;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CreateTextVisualSettings Overload 1 function of VisualSettingPropGenerator class.");
                return null;
            }
        }

        /// <summary>
        /// Generates a TextVisualSettings properties class object and initializes each of its associated properties of the text visual settings with 
        /// default values.  A different visual setting will be stored for each of the various states each property can be accessed in their associated control. 
        /// </summary>
        public static VisualSettingProperties<TextVisualSettings> CreateTextVisualSettings()
        {
            try
            {
                return CreateTextVisualSettings(SystemFonts.DefaultFont, SystemColors.ControlText, ContentAlignment.MiddleCenter, new Point(0, 0), false);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CreateTextVisualSettings Overload 2 function of VisualSettingPropGenerator class.");
                return null;
            }
        }

        #endregion

        #region Image Visual Setting Generation Static Functions

        /// <summary>
        /// Generates an ImageVisualSettings properties class object and initializes each of its associated properties of the image visual settings with 
        /// default values.  A different visual setting will be stored for each of the various states each property can be accessed in their associated control. 
        /// </summary>
        public static VisualSettingProperties<ImageVisualSettings> CreateImageVisualSettings(
                                                                                                    Image DefaultImage, ContentAlignment DefaultImageAlign,
                                                                                                    Point ptDefaultImageOffset, Color DefaultTransparentColor, 
                                                                                                    bool blDefaultTransparent, CoolImageSizeMode DefaultSizeMode, 
                                                                                                    bool DefaultEnableImage)                                                                                                
        {
            try
            {
                VisualSettingProperties<ImageVisualSettings> imgSettingProps = new VisualSettingProperties<ImageVisualSettings>();

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {                    
                    ImageVisualSettings imgVisSetting =
                                        new ImageVisualSettings
                                        {
                                            Image = DefaultImage,
                                            ImageAlign = DefaultImageAlign,
                                            ImageOffset = ptDefaultImageOffset,
                                            TransparentColor = DefaultTransparentColor,
                                            Transparent = blDefaultTransparent,
                                            SizeMode = DefaultSizeMode,
                                            EnableImage = (setting != VisualSettingEnum.Normal) ? DefaultEnableImage : true                                                                    
                                        };

                    imgSettingProps.AddSetting(setting, imgVisSetting);
                }//next i

                return imgSettingProps;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CreateImageVisualSettings Overload 1 function of VisualSettingPropGenerator class.");
                return null;
            }
        }

        /// <summary>
        /// Generates an ImageVisualSettings properties class object and initializes each of its associated properties of the image visual settings with 
        /// default values.  A different visual setting will be stored for each of the various states each property can be accessed in their associated control. 
        /// </summary>
        /// <returns></returns>
        public static VisualSettingProperties<ImageVisualSettings> CreateImageVisualSettings()
        {
            try
            {
                return CreateImageVisualSettings(null, ContentAlignment.MiddleLeft, new Point(0, 0), Color.Transparent, true, CoolImageSizeMode.Normal, false);
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in CreateImageVisualSettings Overload 2 function of VisualSettingPropGenerator class.");
                return null;
            }
        }

        #endregion
    }

    /// <summary>
    /// The GradientVisualSettings class stores a set of properties that contain the most common settings used for gradients in the various
    /// DotCool controls.  
    /// </summary>
    public class GradientVisualSettings
    {
        #region Member Variables
        #endregion

        #region Member Object Variables
        #endregion

        #region Member Data Object Variables

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public GradientVisualSettings()
        {
        }

        #endregion

        #region Gradient Visual Properties

        public Color GradientColor1 { get; set; }
        public Color GradientColor2 { get; set; }
        public CoolGradientType GradientType { get; set; }
        public float GradientSpan { get; set; }
        public bool UseDefaultGradientSpan { get; set; }
        public Point GradientOffset { get; set; }
        public bool DrawGradient { get; set; }

        #endregion
    }

    /// <summary>
    /// The BorderVisualSettings class stores a set of properties that contain the most common settings used for borders in the various
    /// DotCool controls.  
    /// </summary>
    public class BorderVisualSettings
    {
        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public BorderVisualSettings()
        {
        }

        #endregion

        #region Border Appearance Properties

        public CoolShape BorderShape { get; set; }

        public Color BorderColor { get; set; }

        #endregion

        #region Border Size and Position Properties

        public Size BorderSize { get; set; }

        public Point BorderRadius { get; set; }

        public Point BorderOffset { get; set; }

        public int BorderWidth { get; set; }

        #endregion

        #region Border Size and Position Properties (Single Precision)

        public SizeF BorderSizeF { get; set; }

        public PointF BorderRadiusF { get; set; }

        public PointF BorderOffsetF { get; set; }

        public float BorderWidthF { get; set; }

        #endregion
    }

    /// <summary>
    /// The TextVisualSettings class stores a set of properties that contain the most common settings used for text/caption display in the various
    /// DotCool controls.  
    /// </summary>
    public class TextVisualSettings
    {
        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public TextVisualSettings()
        {
        }

        #endregion

        #region Text Visual Properties

        public Font Font { get; set; }
        public Color ForeColor { get; set; }
        public ContentAlignment TextAlign { get; set; }
        public Point TextOffset { get; set; }
        public bool EnableText { get; set; }

        #endregion
    }

    /// <summary>
    /// The ImageVisualSettings class stores a set of properties that contain the most common settings used for display a variety of images in the 
    /// various DotCool controls.  
    /// </summary>
    public class ImageVisualSettings
    {        
        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageVisualSettings()
        {
        }

        #endregion

        #region Image Visual Properties

        public Image Image { get; set; }
        public ContentAlignment ImageAlign { get; set; }
        public Point ImageOffset { get; set; }
        public bool Transparent { get; set; }
        public Color TransparentColor { get; set; }        
        public CoolImageSizeMode SizeMode { get; set; }        
        public bool EnableImage { get; set; }
        
        #endregion
    }
}

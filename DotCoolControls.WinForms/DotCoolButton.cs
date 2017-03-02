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
using System.Collections.Generic;
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
using System.Runtime.InteropServices;

namespace DotCoolControls.WinForms
{
    /// <summary>
    /// The DotCoolButton is a button control that supports button of various shapes, as well as displays stunning visual effects.  Visual effects, include,  
    /// but are not limited to various types customizable gradients, borders, images and text.  Both the effects and functionality of the button can be 
    /// made to be state-specific, such as producing various effects for when the button is in a Normal or Disabled state, as well various types 
    /// of mouse interactions, such as MouseDown and MouseOver events.
    /// </summary>    
    public class DotCoolButton : DotCoolControl
    {
        #region Member Variables
        #endregion

        #region Member Object Variables
        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public DotCoolButton()
            : base()
        {
            try
            {                
                this.SetStyle(ControlStyles.Selectable, true);
                
                SetBorderOffset(new Point(0, 0));

                BorderWidthSettings.BorderWidthMouseDown = 2;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolButton class.");
            }
        }

        #endregion

        #region General Control Drawing/Paint/GDI+ Functions, Event Handlers        

        #endregion

        #region Text Drawing/Painting Functions, Event Handlers

        #endregion

        #region Background Color/Gradient Properties, Functions

        /// <summary>
        /// Background gradient settings of the DotCoolControl when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolBackground"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Background gradient settings of the DotCoolControl when the mouse button is pushed down on the control.")]
        public virtual DotCoolCtlGradientSettings BackGradientSettingsMouseDown
        {
            get
            {

                return GetBackGradientSettingsMouseDown();
            }
        }

        #endregion

        #region Text/Caption Properties, Functions

        /// <summary>
        /// Text/Caption display settings of the DotCoolControl when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolText"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Text/Caption display settings of the DotCoolControl when the mouse button is pushed down on the control.")]
        public virtual DotCoolCtlTextSettings TextSettingsMouseDown
        {
            get
            {

                return GetTextSettings(VisualSettingEnum.MouseDown);
            }
        }

        #endregion

        #region Image Properties, Functions

        /// <summary>
        /// Image display settings of the DotCoolControl when the mouse button is pushed down on the control.
        /// </summary>
        [Browsable(true), Category("CoolImage"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
         Description("Image display settings of the DotCoolControl when the mouse button is pushed down on the control.")]
        public virtual DotCoolCtlImageSettings ImageSettingsMouseDown
        {
            get
            {

                return GetImageSettings(VisualSettingEnum.MouseDown);
            }
        }

        #endregion

        #region Button Size and Position Properties, Functions, Event Handlers               

        /// <summary>
        /// The DotCoolButton will have its control border's sized to the bounds of the control.   Certain classes like the DotCoolButton and DotCoolLabel 
        /// will control the size of the control's border from the DotCoolControl base class in the Resize event.  The control's border are not accessible to
        /// the user in these types of controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            try
            {
                base.OnResize(e);

                SetBorderSize(new Size(this.Width, this.Height));
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnResize function of DotCoolButton class.", "", false);
            }
        }

        #endregion

        #region Control Mouse Interaction Functions, Event Handlers

        #endregion

        #region Button Focus/Activation Properties, Functions

        /// <summary>
        /// Indicates where in the button the focus rectangle will be drawn when the control has focus.
        /// </summary>
        [Browsable(true), Category("Behavior"), DefaultValue(FocusRectEnum.Text),
         Description("Indicates where in the button the focus rectangle will be drawn when the control has focus.")]
        public virtual FocusRectEnum DrawFocusRect
        {
            get
            {
                return GetDrawFocusRect();
            }
            set
            {
                SetDrawFocusRect(value);
                this.Refresh();
            }
        }

        #endregion

        #region Control Cloning and Serialization Properties, Functions

        /// <summary>
        /// Clones all properties and settings of the DotCoolButton control to a new DotCoolButton control.
        /// </summary>
        /// <returns></returns>
        public DotCoolButton Clone()
        {
            try
            {
                DotCoolButton btnClone = new DotCoolButton();
                base.CloneBase(btnClone);
                
                return btnClone;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Clone function of DotCoolButton class.");
                return null;
            }
        }

        #endregion
    }
}

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

namespace DotCoolControls.WinForms
{
    /// <summary>    
    /// The DotCoolRadioButton is a radiobutton control that supports radio boxes and check symbols of various shapes, as well as displays stunning visual
    /// effects.  Visual effects, include, but are not limited to various types customizable gradients, borders, images and text.  In addition, custom images 
    /// can be displayed for the check symbol.  Both the effects and functionality of the radiobutton can be made to be state-specific, such as producing
    /// various effects for when the button is in a Normal or Disabled state, as well various types of mouse interactions, such as MouseOver events.        
    /// </summary>    
    public class DotCoolRadioButton : DotCoolCheckBase
    {
        #region Member Variables

        #endregion

        #region Member Object Variables
        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public DotCoolRadioButton()
            : base()
        {
            try
            {
                m_BorderColorSettings = new DotCoolCheckBaseBorderColorSettings(m_BorderSettingsList);

                VisualSettingProperties<BorderVisualSettings> BorderVisSettings =
                                                VisualSettingPropGenerator.CreateBorderVisualSettings(
                                                                                                        CoolShape.Circle, Color.Transparent, new Size(9, 9),
                                                                                                        new Point(10, 10), new Point(2, 2), 2);

                m_CheckBorderSettingsList = new DotCoolCtlBorderSettingsList(this, true, BorderVisSettings);
                m_CheckBorderColorSettings = new DotCoolCheckBaseBorderColorSettings(m_CheckBorderSettingsList);

                for (VisualSettingEnum setting = VisualSettingEnum.Normal; setting <= VisualSettingEnum.Indeterminate; setting++)
                {
                    m_BorderSettingsList[setting].BorderShape = CoolShape.Circle;
                }//next iSetting

                m_CheckBorderSettingsList[VisualSettingEnum.Normal].BorderColor = Color.Black;

                SelectOnFocus = true;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Constructor function of DotCoolRadioButton class.");
            }
        }

        #endregion

        #region Border Color (State Specific) Appearance Properties, Functions

        /// <summary>
        /// Gets or sets the color of the control's border for each associated control state.
        /// </summary>
        [Browsable(true), Category("CoolBorder"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Gets or sets the color of the control's border for each associated control state.")]
        public new DotCoolCheckBaseBorderColorSettings BorderColorSettings
        {
            get
            {
                return (DotCoolCheckBaseBorderColorSettings)m_BorderColorSettings;
            }
        }

        #endregion

        #region Check Symbol Color (State Specific) Appearance Properties, Functions        

        /// <summary>
        /// Gets or sets the color of the gradient check symbol's border for each associated control state.
        /// </summary>
        [Browsable(true), Category("CoolBorder"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("Gets or sets the color of the gradient check symbol's border for each associated control state.")]
        public virtual DotCoolCheckBaseBorderColorSettings CheckBorderColorSettings
        {
            get
            {
                return m_CheckBorderColorSettings;
            }
        }

        #endregion

        #region Radio Button Selection/Value Setting Properties, Functions        

        /// <summary>
        /// Indicates if the radio button will be selected when it receives the focus.
        /// </summary>
        [Browsable(true), Category("Behavior"), DefaultValue(true),
         Description("Indicates if the radio button will be selected when it receives the focus.")]
        public virtual bool SelectOnFocus
        {
            get
            {
                return m_blSelectOnFocus;
            }
            set
            {
                m_blSelectOnFocus = value;
            }
        }
    
        /// <summary>
        /// Gets/Sets a value indicating if the check control is checked or unchecked.
        /// </summary>
        [Browsable(true), Category("CoolCheck"),
         Description("Gets/Sets a value indicating if the check control is checked or unchecked.")]
        public override bool Checked
        {
            get
            {
                return base.Checked;
            }

            set
            {
                base.Checked = value;

                if(value)                 
                    UpdateGroupSelection();                   
            }
        }

        /// <summary>
        /// Updates the state of all DotCoolRadioButton controls that are contained in the same container as the DotCoolRadioButton control that 
        /// is selected.  Only one radio button in the container can be selected at a time.
        /// </summary>
        protected virtual void UpdateGroupSelection()
        {
            try
            {
                if (this.Parent != null)
                {
                    foreach (DotCoolRadioButton rbCtl in this.Parent.Controls.OfType<DotCoolRadioButton>())
                    {
                        if (rbCtl != this && rbCtl.Checked)
                        {
                            rbCtl.Checked = false;
                        }//next rbCtl
                    }//next rbCtl
                }//end if
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in UpdateGroupSelection function of DotCoolRadioButton class.");
            }
        }

        #endregion

        #region Control Mouse Interaction Functions, Event Handlers       

        /// <summary>
        /// Raises the Click event of the control.  When the control is clicked, the radio button will be selected and the previously selected radio button 
        /// in the group (container control) will be deselected.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            try
            {
                bool blChangeCheckStatus = false;

                if (m_blMouseDown)
                    blChangeCheckStatus = true;

                base.OnClick(e);

                if (blChangeCheckStatus)
                {
                    this.Checked = true;               
                }//end if                
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnClick function of DotCoolRadioButton class.", "", true);
            }
        }

        /// <summary>
        /// The DotCoolRadioButton will not raise a DoubleClick event, so a double click on the control will result in the control raising a Click event, instead.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {            
            OnClick(e);
        }

        #endregion

        #region Control Focus Functions, Event Handlers

        /// <summary>
        /// When the radio button control receives the focus and the SelectOnFocus property is set to true, the active radio button will be selected.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            try
            {
                base.OnGotFocus(e);

                if (m_blSelectOnFocus)
                {                    
                    this.Checked = true;
                }//end if

                this.Refresh();
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in OnGotFocus function of DotCoolRadioButton class.", "", true);
            }
        }

        #endregion

        #region Control Cloning and Serialization Properties, Functions

        /// <summary>
        /// Clones all properties and settings of the DotCoolRadioButton control to a new DotCoolRadioButton control.
        /// </summary>
        /// <returns></returns>
        public DotCoolRadioButton Clone()
        {
            try
            {
                DotCoolRadioButton rbClone = new DotCoolRadioButton();
                base.CloneBase(rbClone);

                rbClone.SelectOnFocus = SelectOnFocus;

                return rbClone;
            }
            catch (Exception err)
            {
                ErrorHandler.ShowErrorMessage(err, "Error in Clone function of DotCoolRadioButton class.");
                return null;
            }
        }

        #endregion
    }
}

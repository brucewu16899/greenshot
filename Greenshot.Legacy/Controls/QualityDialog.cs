//  Greenshot - a free and open source screenshot tool
//  Copyright (C) 2007-2017 Thomas Braun, Jens Klingen, Robin Krom
// 
//  For more information see: http://getgreenshot.org/
//  The Greenshot project is hosted on GitHub: https://github.com/greenshot
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 1 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

#region Usings

using System;
using Greenshot.Core;
using Greenshot.Core.Configuration;

#endregion

namespace Greenshot.Legacy.Controls
{
	/// <summary>
	///     Description of JpegQualityDialog.
	/// </summary>
	public partial class QualityDialog : GreenshotForm
	{
		private readonly IOutputConfiguration _outputConfiguration;

		public QualityDialog(SurfaceOutputSettings outputSettings, IOutputConfiguration outputConfiguration = null)
		{
			_outputConfiguration = outputConfiguration;
			Settings = outputSettings;
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			checkBox_reduceColors.Checked = Settings.ReduceColors;
			trackBarJpegQuality.Enabled = OutputFormat.jpg.Equals(outputSettings.Format);
			trackBarJpegQuality.Value = Settings.JPGQuality;
			textBoxJpegQuality.Enabled = OutputFormat.jpg.Equals(outputSettings.Format);
			textBoxJpegQuality.Text = Settings.JPGQuality.ToString();
			ToFront = true;
		}

		public SurfaceOutputSettings Settings { get; set; }

		private void Button_okClick(object sender, EventArgs e)
		{
			Settings.JPGQuality = trackBarJpegQuality.Value;
			Settings.ReduceColors = checkBox_reduceColors.Checked;
			if (checkbox_dontaskagain.Checked && _outputConfiguration != null)
			{
				_outputConfiguration.OutputFileJpegQuality = Settings.JPGQuality;
				_outputConfiguration.OutputFilePromptQuality = false;
				_outputConfiguration.OutputFileReduceColors = Settings.ReduceColors;
			}
		}

		private void TrackBarJpegQualityScroll(object sender, EventArgs e)
		{
			textBoxJpegQuality.Text = trackBarJpegQuality.Value.ToString();
		}
	}
}
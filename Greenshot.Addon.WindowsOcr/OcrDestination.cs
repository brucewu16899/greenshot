﻿//  Greenshot - a free and open source screenshot tool
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
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Dapplo.Log;
using Greenshot.Addon.Interfaces;
using Greenshot.Addon.Interfaces.Destination;
using Greenshot.CaptureCore;
using Greenshot.CaptureCore.Extensions;
using Greenshot.Core;
using Greenshot.Core.Interfaces;
using MahApps.Metro.IconPacks;

#endregion

namespace Greenshot.Addon.WindowsOcr
{
	[Destination(OcrDesignation)]
	[PartNotDiscoverable]
	public sealed class OcrDestination : AbstractDestination
	{
		private const string OcrDesignation = "Ocr";
		private static readonly LogSource Log = new LogSource();

		private async Task<INotification> ExportCaptureAsync(ICapture capture, CancellationToken token = default(CancellationToken))
		{
			var returnValue = new Notification
			{
				NotificationType = NotificationTypes.Success,
				Source = OcrDesignation,
				SourceType = SourceTypes.Destination,
				Text = OcrDesignation
			};

			try
			{
				var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
				using (var imageStream = new MemoryStream())
				{
					capture.SaveToStream(imageStream, new SurfaceOutputSettings());
					imageStream.Position = 0;

					var decoder = await BitmapDecoder.CreateAsync(imageStream.AsRandomAccessStream());
					var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

					var ocrResult = await ocrEngine.RecognizeAsync(softwareBitmap);
					ClipboardHelper.SetClipboardData(ocrResult.Text);
				}
			}
			catch (TaskCanceledException tcEx)
			{
				returnValue.Text = "Share cancelled.";
				returnValue.NotificationType = NotificationTypes.Cancel;
				returnValue.ErrorText = tcEx.Message;
				Log.Info().WriteLine(tcEx.Message);
			}
			catch (Exception e)
			{
				returnValue.Text = "Share failed.";
				returnValue.NotificationType = NotificationTypes.Fail;
				returnValue.ErrorText = e.Message;
				Log.Warn().WriteLine(e, "Share export failed");
			}
			return returnValue;
		}

		/// <summary>
		///     Setup
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
			Designation = OcrDesignation;
			Export = async (exportContext, capture, token) => await ExportCaptureAsync(capture, token);
			Text = OcrDesignation;
			Icon = new PackIconModern
			{
				// TODO: Search icon
				Kind = PackIconModernKind.BookOpenText
			};

			var languages = OcrEngine.AvailableRecognizerLanguages;
			foreach (var language in languages)
			{
				Log.Info().WriteLine("Found language {0} {1}", language.NativeName, language.LanguageTag);
			}
		}
	}
}
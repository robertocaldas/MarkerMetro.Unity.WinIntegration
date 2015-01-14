﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#elif NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endif

namespace MarkerMetro.Unity.WinIntegration.VideoPlayer
{
    public enum VideoStretch
    {
        // Summary:
        //     The content preserves its original size.
        None = 0,
        // Summary:
        //     The content is resized to fill the destination dimensions. The aspect ratio
        //     is not preserved.
        Fill = 1,
        // Summary:
        //     The content is resized to fit in the destination dimensions while it preserves
        //     its native aspect ratio.
        Uniform = 2,
        // Summary:
        //     The content is resized to fill the destination dimensions while it preserves
        //     its native aspect ratio. If the aspect ratio of the destination rectangle
        //     differs from the source, the source content is clipped to fit in the destination
        //     dimensions.
        UniformToFill = 3,
    }

    public static class VideoPlayer
    {
        /// <summary>
        /// Get the state of Video is currently playing or not.
        /// </summary>
        public static bool IsPlaying { get; private set; }

#if WINDOWS_PHONE || NETFX_CORE
        /// <summary>
        /// Callback on video ended.
        /// </summary>
        private static Action _onVideoEnded;

        private static MediaElement _videoElement;
        private static Popup _videoPopup;
#endif

        /// <summary>
        /// Initialize VideoPlayer.
        /// </summary>
        public static void Initialize(VideoStretch stretch = VideoStretch.None)
        {
#if WINDOWS_PHONE || NETFX_CORE
            Dispatcher.InvokeOnUIThread(() =>
            {
                _videoPopup = new Popup();

                _videoPopup.VerticalOffset = 0;
                _videoPopup.HorizontalOffset = 0;

                _videoElement = new MediaElement();
                _videoPopup.Child = _videoElement;

                _videoElement.MediaEnded += _videoElement_MediaEnded;
                _videoElement.MediaOpened += _videoElement_MediaOpened;

#if WINDOWS_PHONE
                _videoPopup.Height = Application.Current.Host.Content.ActualHeight;
                _videoPopup.Width = Application.Current.Host.Content.ActualWidth;

                _videoElement.Tap += _videoElement_Tap;
                _videoElement.Stretch = (Stretch)stretch;
#else
                _videoPopup.Height = Window.Current.Bounds.Height;
                _videoPopup.Width = Window.Current.Bounds.Width;

                _videoElement.Tapped += _videoElement_Tapped;
                _videoElement.Stretch = (Stretch)stretch;
#endif

                _videoElement.AutoPlay = false;

                _videoElement.Height = _videoPopup.Height;
                _videoElement.Width = _videoPopup.Width;

                _videoPopup.IsOpen = true;
                _videoPopup.Visibility = Visibility.Collapsed;
                _videoElement.Visibility = Visibility.Collapsed;
            });
#endif
        }

#if WINDOWS_PHONE || NETFX_CORE
        /// <summary>
        /// Event handler for user tap when video is playing.
        /// </summary>
#if WINDOWS_PHONE
        static void _videoElement_Tap(object sender, System.Windows.Input.GestureEventArgs e)
#else
        static void _videoElement_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
#endif
        {
            StopVideo();
        }

        static void _videoElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            _videoElement.Play();
        }

        static void _videoElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            StopVideo();
        }
#endif

        /// <summary>
        /// Plays the video given path of the file.
        /// </summary>
        public static void PlayVideo(string filename, Action onVideoEnded)
        {
#if WINDOWS_PHONE || NETFX_CORE
            Dispatcher.InvokeOnUIThread(() =>
            {
                if (_videoPopup == null || _videoElement == null)
                {
                    throw new Exception("VideoPlayer not initialized");
                }

                IsPlaying = true;
                _videoElement.Visibility = Visibility.Visible;
                _videoPopup.Visibility = Visibility.Visible;

#if NETFX_CORE
                _videoPopup.Height = Window.Current.Bounds.Height;
                _videoPopup.Width = Window.Current.Bounds.Width;
                _videoElement.Height = _videoPopup.Height;
                _videoElement.Width = _videoPopup.Width;
#endif

                _onVideoEnded = onVideoEnded;
                _videoElement.Source = new Uri(filename, UriKind.Absolute);
            });
#else
            throw new PlatformNotSupportedException("");
#endif
        }

        /// <summary>
        /// Stop playing the video.
        /// </summary>
        public static void StopVideo ()
        {
#if WINDOWS_PHONE || NETFX_CORE
            Dispatcher.InvokeOnUIThread(() =>
            {
                if (_videoPopup == null || _videoElement == null)
                {
                    throw new Exception("VideoPlayer not initialized");
                }

                _videoElement.Stop();
                _videoElement.Visibility = Visibility.Collapsed;
                _videoPopup.Visibility = Visibility.Collapsed;

                IsPlaying = false;

                if (_onVideoEnded != null)
                {
                    Dispatcher.InvokeOnAppThread(() =>
                    {
                        _onVideoEnded();
                    });
                }
            });
#else
            throw new PlatformNotSupportedException("");
#endif
        }
    }
}

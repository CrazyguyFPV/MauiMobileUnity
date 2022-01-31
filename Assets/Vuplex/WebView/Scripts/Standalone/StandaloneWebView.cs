/**
* Copyright (c) 2021 Vuplex Inc. All rights reserved.
*
* Licensed under the Vuplex Commercial Software Library License, you may
* not use this file except in compliance with the License. You may obtain
* a copy of the License at
*
*     https://vuplex.com/commercial-library-license
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
// For now, ignore the warning that 3D WebView's
// callback-based APIs for legacy .NET are deprecated.
#pragma warning disable CS0618
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Vuplex.WebView.Internal;

#if NET_4_6 || NET_STANDARD_2_0
    using System.Threading.Tasks;
#endif

namespace Vuplex.WebView {

    /// <summary>
    /// The base IWebView implementation used by 3D WebView for Windows and macOS.
    /// This class also includes extra methods for Standalone-specific functionality.
    /// </summary>
    public abstract partial class StandaloneWebView : BaseWebView,
                                                      IWithDownloads,
                                                      IWithFileSelection,
                                                      IWithKeyDownAndUp,
                                                      IWithKeyModifiers,
                                                      IWithMovablePointer,
                                                      IWithMutableAudio,
                                                      IWithPointerDownAndUp,
                                                      IWithPopups {

        /// <summary>
        /// Indicates that a server requested [HTTP authentication](https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication)
        /// to make the browser show its built-in authentication UI.
        /// </summary>
        /// <remarks>
        /// If no handler is attached to this event, then the host's authentication request will be ignored
        /// and the page will not be paused. If a handler is attached to this event, then the page will
        /// be paused until Continue() or Cancel() is called.
        ///
        /// You can test basic HTTP auth using [this page](https://jigsaw.w3.org/HTTP/Basic/)
        /// with the username "guest" and the password "guest".
        /// </remarks>
        /// <remarks>
        /// This event is not raised for most websites because most sites implement a custom sign-in page
        /// instead of using HTTP authentication to show the browser's built-in authentication UI.
        /// </remarks>
        /// <example>
        /// <code>
        /// await webViewPrefab.WaitUntilInitialized();
        /// #if UNITY_STANDALONE
        ///     var standaloneWebView = webViewPrefab.WebView as StandaloneWebView;
        ///     standaloneWebView.AuthRequested += (sender, eventArgs) => {
        ///         Debug.Log("Auth requested by " + eventArgs.Host);
        ///         eventArgs.Continue("myUsername", "myPassword");
        ///     };
        /// #endif
        /// </code>
        /// </example>
        public event EventHandler<AuthRequestedEventArgs> AuthRequested {
            add {
                if (_authRequestedHandler != null) {
                    throw new InvalidOperationException("AuthRequested supports only one event handler. Please remove the existing handler before adding a new one.");
                }
                _authRequestedHandler = value;
                WebView_setAuthEnabled(_nativeWebViewPtr, true);
            }
            remove {
                if (_authRequestedHandler == value) {
                    _authRequestedHandler = null;
                    WebView_setAuthEnabled(_nativeWebViewPtr, false);
                }
            }
        }

        /// <see cref="IWithDownloads"/>
        public event EventHandler<DownloadChangedEventArgs> DownloadProgressChanged;

        /// <see cref="IWithFileSelection"/>
        public event EventHandler<FileSelectionEventArgs> FileSelectionRequested {
            add {
                if (_fileSelectionHandler != null) {
                    throw new InvalidOperationException("FileSelectionRequested supports only one event handler. Please remove the existing handler before adding a new one.");
                }
                _fileSelectionHandler = value;
                WebView_setFileSelectionEnabled(_nativeWebViewPtr, true);
            }
            remove {
                if (_fileSelectionHandler == value) {
                    _fileSelectionHandler = null;
                    WebView_setFileSelectionEnabled(_nativeWebViewPtr, false);
                }
            }
        }

        /// <see cref="IWithPopups"/>
        public event EventHandler<PopupRequestedEventArgs> PopupRequested;

        public override void CanGoBack(Action<bool> callback) {

            base.CanGoBack(callback);
            OnCanGoBack();
        }

        public override void CanGoForward(Action<bool> callback) {

            base.CanGoForward(callback);
            OnCanGoForward();
        }

        public override void CaptureScreenshot(Action<byte[]> callback) {

            base.CaptureScreenshot(callback);
            OnCaptureScreenshot();
        }

        public static void ClearAllData() {

            var pluginIsInitialized = WebView_pluginIsInitialized();
            if (pluginIsInitialized) {
                _throwAlreadyInitializedException("clear the browser data", "ClearAllData");
            }
            var cachePath = _getCachePath();
            if (Directory.Exists(cachePath)) {
                Directory.Delete(cachePath, true);
            }
        }

        public override void Copy() {

            _assertValidState();
            WebView_copy(_nativeWebViewPtr);
            OnCopy();
        }

        public override void Cut() {

            _assertValidState();
            WebView_cut(_nativeWebViewPtr);
            OnCut();
        }

        /// <summary>
        /// Dispatches a touch event (i.e. touchstart, touchend, touchmove, touchcancel) in
        /// the webview. This can be used, for example, to implement multitouch interactions.
        /// </summary>
        /// <example>
        /// <code>
        /// #if UNITY_STANDALONE
        ///     var standaloneWebView = webViewPrefab.WebView as StandaloneWebView;
        ///     // Start and end a first touch at (250px, 100px) in the web page.
        ///     var normalizedPoint1 = new Vector2(250, 100) / webViewPrefab.WebView.SizeInPixels;
        ///     standaloneWebView.DispatchTouchEvent(new TouchEvent {
        ///         TouchID = 1,
        ///         Point = normalizedPoint1,
        ///         Type = TouchEventType.Start
        ///     });
        ///     standaloneWebView.DispatchTouchEvent(new TouchEvent {
        ///         TouchID = 1,
        ///         Point = normalizedPoint1,
        ///         Type = TouchEventType.End
        ///     });
        ///     // Start a second touch at (400px, 100px), move it to (410px, 100), and release it.
        ///     var normalizedPoint2 = new Vector2(400, 100) / webViewPrefab.WebView.SizeInPixels;
        ///     standaloneWebView.DispatchTouchEvent(new TouchEvent {
        ///         TouchID = 2,
        ///         Point = normalizedPoint2,
        ///         Type = TouchEventType.Start
        ///     });
        ///     var normalizedPoint3 = new Vector2(410, 100) / webViewPrefab.WebView.SizeInPixels;
        ///     standaloneWebView.DispatchTouchEvent(new TouchEvent {
        ///         TouchID = 2,
        ///         Point = normalizedPoint3,
        ///         Type = TouchEventType.Move
        ///     });
        ///     standaloneWebView.DispatchTouchEvent(new TouchEvent {
        ///         TouchID = 2,
        ///         Point = normalizedPoint3,
        ///         Type = TouchEventType.End
        ///     });
        /// #endif
        /// </code>
        /// </example>
        public void DispatchTouchEvent(TouchEvent touchEvent) {

            _assertValidState();
            float pointX = touchEvent.Point.x * _nativeWidth;
            float pointY = touchEvent.Point.y * _nativeHeight;
            WebView_dispatchTouchEvent(
                _nativeWebViewPtr,
                touchEvent.TouchID,
                (int)touchEvent.Type,
                pointX,
                pointY,
                touchEvent.RadiusX,
                touchEvent.RadiusY,
                touchEvent.RotationAngle,
                touchEvent.Pressure
            );
        }

        /// <summary>
        /// Like Web.EnableRemoteDebugging(), but starts the DevTools session on the
        /// specified port instead of the default port 8080.
        /// </summary>
        /// <param name="portNumber">Port number in the range 1024 - 65535.</param>
        /// <example>
        /// <code>
        /// void Awake() {
        ///     StandaloneWebView.EnableRemoteDebugging(9000);
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="Web.EnableRemoteDebugging"/>
        public static void EnableRemoteDebugging(int portNumber) {

            if (!(1024 <= portNumber && portNumber <= 65535)) {
                throw new ArgumentException(string.Format("The given port number ({0}) is not in the range from 1024 to 65535.", portNumber));
            }
            var success = WebView_enableRemoteDebugging(portNumber);
            if (!success) {
                _throwAlreadyInitializedException("enable remote debugging", "EnableRemoteDebugging");
            }
        }

        public override void ExecuteJavaScript(string javaScript, Action<string> callback) {

            base.ExecuteJavaScript(javaScript, callback);
            OnExecuteJavaScript();
        }

    #if NET_4_6 || NET_STANDARD_2_0
        /// <summary>
        /// Gets the cookie that matches the given URL and cookie name, or
        /// null if no cookie matches.
        /// </summary>
        /// <remarks>
        /// This method can only be called after
        /// one or more webviews have been initialized.
        /// </remarks>
        public static Task<Cookie> GetCookie(string url, string cookieName) {

            var task = new TaskCompletionSource<Cookie>();
            GetCookie(url, cookieName, task.SetResult);
            return task.Task;
        }
    #endif

        /// <summary>
        /// Like the other version of GetCookie(), except it uses a callback
        /// instead of a Task in order to be compatible with legacy .NET.
        /// </summary>
        public static void GetCookie(string url, string cookieName, Action<Cookie> callback) {

            var pluginIsInitialized = WebView_pluginIsInitialized();
            if (!pluginIsInitialized) {
                throw new InvalidOperationException("On Windows and macOS, GetCookie() can only be called when the Chromium process is running (i.e. after a webview is initialized).");
            }
            var resultCallbackId = Guid.NewGuid().ToString();
            _pendingGetCookieResultCallbacks[resultCallbackId] = callback;
            WebView_getCookie(url, cookieName, resultCallbackId);
        }

    #if NET_4_6 || NET_STANDARD_2_0
        /// <summary>
        /// Gets all of the cookies that match the given URL.
        /// </summary>
        /// <remarks>
        /// This method can only be called after
        /// one or more webviews have been initialized.
        /// </remarks>
        public static Task<Cookie[]> GetCookies(string url) {

            var task = new TaskCompletionSource<Cookie[]>();
            GetCookies(url, task.SetResult);
            return task.Task;
        }
    #endif

        /// <summary>
        /// Like the other version of GetCookies(), except it uses a callback
        /// instead of a Task in order to be compatible with legacy .NET.
        /// </summary>
        public static void GetCookies(string url, Action<Cookie[]> callback) {

            var pluginIsInitialized = WebView_pluginIsInitialized();
            if (!pluginIsInitialized) {
                throw new InvalidOperationException("On Windows and macOS, GetCookies() can only be called when the Chromium process is running (i.e. after a webview is initialized).");
            }
            var resultCallbackId = Guid.NewGuid().ToString();
            _pendingGetCookiesResultCallbacks[resultCallbackId] = callback;
            WebView_getCookies(url, resultCallbackId);
        }

        public override void GetRawTextureData(Action<byte[]> callback) {

            base.GetRawTextureData(callback);
            OnGetRawTextureData();
        }

        public static void GloballySetUserAgent(bool mobile) {

            var success = WebView_globallySetUserAgentToMobile(mobile);
            if (!success) {
                throw new InvalidOperationException(USER_AGENT_EXCEPTION_MESSAGE);
            }
        }

        public static void GloballySetUserAgent(string userAgent) {

            var success = WebView_globallySetUserAgent(userAgent);
            if (!success) {
                throw new InvalidOperationException(USER_AGENT_EXCEPTION_MESSAGE);
            }
        }

        public override void GoBack() {

            base.GoBack();
            OnGoBack();
        }

        public override void GoForward() {

            base.GoForward();
            OnGoForward();
        }

        [Obsolete("The IWithKeyModifiers interface is now deprecated. Please use the IWithKeyDownAndUp interface instead.")]
        public void HandleKeyboardInput(string key, KeyModifier modifiers) {

            KeyDown(key, modifiers);
            KeyUp(key, modifiers);
        }

        public override void Init(Texture2D viewportTexture, float width, float height, Texture2D videoTexture) {

            base.Init(viewportTexture, width, height, videoTexture);
            _nativeWebViewPtr = WebView_new(gameObject.name, _nativeWidth, _nativeHeight, null);
            if (_nativeWebViewPtr == IntPtr.Zero) {
                throw new WebViewUnavailableException("Failed to instantiate a new webview. This could indicate that you're using an expired trial version of 3D WebView.");
            }
        }

        /// <see cref="IWithKeyDownAndUp"/>
        public void KeyDown(string key, KeyModifier modifiers) {

            _assertValidState();
            WebView_keyDown(_nativeWebViewPtr, key, (int)modifiers);
        }

        /// <see cref="IWithKeyDownAndUp"/>
        public void KeyUp(string key, KeyModifier modifiers) {

            _assertValidState();
            WebView_keyUp(_nativeWebViewPtr, key, (int)modifiers);
        }

        public override void LoadHtml(string html) {

            base.LoadHtml(html);
            OnLoadHtml();
        }

        public override void LoadUrl(string url) {

            base.LoadUrl(url);
            OnLoadUrl(url);
        }

        public override void LoadUrl(string url, Dictionary<string, string> additionalHttpHeaders) {

            if (additionalHttpHeaders != null) {
                foreach (var headerName in additionalHttpHeaders.Keys) {
                    if (headerName.Equals("Accept-Language", StringComparison.InvariantCultureIgnoreCase)) {
                        WebViewLogger.LogError("On Windows and macOS, the Accept-Language request header cannot be set with LoadUrl(url, headers). For more info, please see this article: <em>https://support.vuplex.com/articles/how-to-change-accept-language-header</em>");
                    }
                }
            }
            base.LoadUrl(url, additionalHttpHeaders);
        }

        /// <see cref="IWithMovablePointer"/>
        public void MovePointer(Vector2 point) {

            _assertValidState();
            int nativeX = (int) (point.x * _nativeWidth);
            int nativeY = (int) (point.y * _nativeHeight);
            WebView_movePointer(_nativeWebViewPtr, nativeX, nativeY);
        }

        public override void Paste() {

            _assertValidState();
            WebView_paste(_nativeWebViewPtr);
            OnPaste();
        }

        /// <see cref="IWithPointerDownAndUp"/>
        public void PointerDown(Vector2 point) {

            _pointerDown(point, MouseButton.Left, 1);
        }

        /// <see cref="IWithPointerDownAndUp"/>
        public void PointerDown(Vector2 point, PointerOptions options) {

            if (options == null) {
                options = new PointerOptions();
            }
            _pointerDown(point, options.Button, options.ClickCount);
        }

        /// <see cref="IWithPointerDownAndUp"/>
        public void PointerUp(Vector2 point) {

            _pointerUp(point, MouseButton.Left, 1);
        }

        /// <see cref="IWithPointerDownAndUp"/>
        public void PointerUp(Vector2 point, PointerOptions options) {

            if (options == null) {
                options = new PointerOptions();
            }
            _pointerUp(point, options.Button, options.ClickCount);
        }

        public override void SelectAll() {

            _assertValidState();
            WebView_selectAll(_nativeWebViewPtr);
        }

        /// <summary>
        /// By default, web pages cannot access the device's
        /// camera or microphone via JavaScript.
        /// Invoking `SetAudioAndVideoCaptureEnabled(true)` allows
        /// **all web pages** to access the camera and microphone.
        /// </summary>
        /// <remarks>
        /// This is useful, for example, to enable WebRTC support.
        /// This method can only be called prior to initializing any webviews.
        /// </remarks>
        /// <example>
        /// <code>
        /// void Awake() {
        ///     #if UNITY_STANDALONE
        ///         StandaloneWebView.SetAudioAndVideoCaptureEnabled(true);
        ///     #endif
        /// }
        /// </code>
        /// </example>
        public static void SetAudioAndVideoCaptureEnabled(bool enabled) {

            var success = WebView_setAudioAndVideoCaptureEnabled(enabled);
            if (!success) {
                _throwAlreadyInitializedException("enable audio and video capture", "SetAudioAndVideoCaptureEnabled");
            }
        }

        /// <see cref="IWithMutableAudio"/>
        public void SetAudioMuted(bool muted) {

            _assertValidState();
            WebView_setAudioMuted(_nativeWebViewPtr, muted);
        }

        public static void SetAutoplayEnabled(bool enabled) {

            var success = WebView_setAutoplayEnabled(enabled);
            if (!success) {
                _throwAlreadyInitializedException("enable autoplay", "SetAutoplayEnabled");
            }
        }

        /// <summary>
        /// By default, Chromium's cache is saved at the file path Application.persistentDataPath/Vuplex.WebView/chromium-cache,
        /// but you can call this method to specify a custom file path for the cache instead. This is useful, for example, to
        /// allow multiple instances of your app to run on the same machine, because multiple instances of Chromium cannot
        /// simultaneously share the same cache. This method can only be called prior to initializing any webviews.
        /// </summary>
        /// <example>
        /// <code>
        /// void Awake() {
        ///     #if UNITY_STANDALONE
        ///         var customCachePath = Path.Combine(Application.persistentDataPath, "your-chromium-cache");
        ///         StandaloneWebView.SetCachePath(customCachePath);
        ///     #endif
        /// }
        /// </code>
        /// </example>
        public static void SetCachePath(string absoluteFilePath) {

            _cachePathOverride = absoluteFilePath;
            _setCachePath(absoluteFilePath, "SetCachePath");
        }

        /// <summary>
        /// Sets additional command line arguments to pass to Chromium.
        /// <summary>
        /// <remarks>
        /// [Here's an unofficial list of Chromium command line arguments](https://peter.sh/experiments/chromium-command-line-switches/).
        /// This method can only be called prior to initializing any webviews.
        /// </remarks>
        /// <example>
        /// <code>
        /// void Awake() {
        ///     #if UNITY_STANDALONE
        ///         StandaloneWebView.SetCommandLineArguments("--ignore-certificate-errors --disable-web-security");
        ///     #endif
        /// }
        /// </code>
        /// </example>
        public static void SetCommandLineArguments(string args) {

            var success = WebView_setCommandLineArguments(args);
            if (!success) {
                _throwAlreadyInitializedException("set command line arguments", "SetCommandLineArguments");
            }
        }

    #if NET_4_6 || NET_STANDARD_2_0
        /// <summary>
        /// Sets the given cookie and returns a Task&lt;bool&gt; indicating
        /// whether the cookie was set successfully.
        /// </summary>
        /// <remarks>
        /// If setting the cookie fails, it could be because the data in the provided Cookie
        /// was malformed. For more info regarding the failure, check the logs.
        /// This method can only be called after one or more webviews have been initialized.
        /// </remarks>
        /// <example>
        /// <code>
        /// var success = await StandaloneWebView.SetCookie(new Cookie {
        ///     Domain = "vuplex.com",
        ///     Path = "/",
        ///     Name = "example_name",
        ///     Value = "example_value",
        ///     Secure = true,
        ///     // Expire one day from now
        ///     ExpirationDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds() + 60 * 60 * 24
        /// });
        /// </code>
        /// </example>
        public static Task<bool> SetCookie(Cookie cookie) {

            var task = new TaskCompletionSource<bool>();
            SetCookie(cookie, task.SetResult);
            return task.Task;
        }
    #endif

        /// <summary>
        /// Like the other version of SetCookie(), except it uses a callback
        /// instead of a Task in order to be compatible with legacy .NET.
        /// </summary>
        public static void SetCookie(Cookie cookie, Action<bool> callback) {

            var pluginIsInitialized = WebView_pluginIsInitialized();
            if (!pluginIsInitialized) {
                throw new InvalidOperationException("On Windows and macOS, SetCookie() can only be called when the Chromium process is running (i.e. after a webview is initialized).");
            }
            if (cookie == null) {
                throw new ArgumentException("Cookie cannot be null.");
            }
            if (!cookie.IsValid) {
                throw new ArgumentException("Cannot set invalid cookie: " + cookie);
            }
            var resultCallbackId = Guid.NewGuid().ToString();
            _pendingSetCookieResultCallbacks[resultCallbackId] = callback;
            WebView_setCookie(cookie.ToJson(), resultCallbackId);
        }

        /// <see cref="IWithDownloads"/>
        public void SetDownloadsEnabled(bool enabled) {

            _assertValidState();
            var downloadsDirectoryPath = enabled ? Path.Combine(Application.temporaryCachePath, Path.Combine("Vuplex.WebView", "downloads")) : "";
            WebView_setDownloadsEnabled(_nativeWebViewPtr, downloadsDirectoryPath);
        }

        public static void SetIgnoreCertificateErrors(bool ignore) {

            var success = WebView_setIgnoreCertificateErrors(ignore);
            if (!success) {
                _throwAlreadyInitializedException("ignore certificate errors", "SetIgnoreCertificateErrors");
            }
        }

        /// <summary>
        /// By default, the native file picker for file input elements is disabled,
        /// but it can be enabled with this method.
        /// </summary>
        /// <example>
        /// <code>
        /// await webViewPrefab.WaitUntilInitialized();
        /// #if UNITY_STANDALONE
        ///     var standaloneWebView = webViewPrefab.WebView as StandaloneWebView;
        ///     standaloneWebView.SetNativeFileDialogEnabled(true);
        /// #endif
        /// </code>
        /// </example>
        public void SetNativeFileDialogEnabled(bool enabled) {

            _assertValidState();
            WebView_setNativeFileDialogEnabled(_nativeWebViewPtr, enabled);
        }

        public void SetPopupMode(PopupMode popupMode) {

            WebView_setPopupMode(_nativeWebViewPtr, (int)popupMode);
        }

        /// <summary>
        /// By default, web pages cannot share the device's screen
        /// via JavaScript. Invoking `SetScreenSharingEnabled(true)` allows
        /// **all web pages** to share the screen.
        /// </summary>
        /// <remarks>
        /// The screen that is shared is the default screen, and there isn't currently
        /// support for sharing a different screen or a specific application window.
        /// This is a limitation of Chromium Embedded Framework (CEF), which 3D WebView
        /// uses to embed Chromium. Also, this method can only be called prior to
        /// initializing any webviews.
        /// </remarks>
        /// <example>
        /// <code>
        /// void Awake() {
        ///     #if UNITY_STANDALONE
        ///         StandaloneWebView.SetScreenSharingEnabled(true);
        ///     #endif
        /// }
        /// </code>
        /// </example>
        public static void SetScreenSharingEnabled(bool enabled) {

            var success = WebView_setScreenSharingEnabled(enabled);
            if (!success) {
                _throwAlreadyInitializedException("enable or disable screen sharing", "SetScreenSharingEnabled");
            }
        }

        public static void SetStorageEnabled(bool enabled) {

            var cachePath = enabled ? _getCachePath() : "";
            _setCachePath(cachePath, "SetStorageEnabled");
        }

        /// <summary>
        /// Sets the target web frame rate. The default is `60`, which is also the maximum value.
        /// Specifying a target frame rate of `0` disables the frame rate limit. This method can only be called prior
        /// to initializing any webviews.
        /// </summary>
        /// <example>
        /// <code>
        /// void Awake() {
        ///     #if UNITY_STANDALONE
        ///         // Disable the frame rate limit.
        ///         StandaloneWebView.SetTargetFrameRate(0);
        ///     #endif
        /// }
        /// </code>
        /// </example>
        public static void SetTargetFrameRate(uint targetFrameRate) {

            var success = WebView_setTargetFrameRate(targetFrameRate);
            if (!success) {
                _throwAlreadyInitializedException("set the target frame rate", "SetTargetFrameRate");
            }
        }

        /// <summary>
        /// Sets the zoom level to the specified value. Specify `0.0` to reset the zoom level.
        /// </summary>
        /// <example>
        /// <code>
        /// // Set the zoom level to 1.75 after the page finishes loading.
        /// await webViewPrefab.WaitUntilInitialized();
        /// webViewPrefab.WebView.LoadProgressChanged += (sender, eventArgs) => {
        ///     if (eventArgs.Type == ProgressChangeType.Finished) {
        ///         #if UNITY_STANDALONE
        ///             var standaloneWebView = webViewPrefab.WebView as StandaloneWebView;
        ///             standaloneWebView.SetZoomLevel(1.75f);
        ///         #endif
        ///     }
        /// };
        /// </code>
        /// </example>
        public void SetZoomLevel(float zoomLevel) {

            _assertValidState();
            WebView_setZoomLevel(_nativeWebViewPtr, zoomLevel);
        }

        public static void TerminatePlugin() {

            WebView_terminatePlugin();
        }

        delegate void GetCookieCallback(string requestId, string serializedCookies);
        delegate void SetCookieCallback(string requestId, bool success);
        delegate void UnitySendMessageFunction(string gameObjectName, string methodName, string message);

        EventHandler<AuthRequestedEventArgs> _authRequestedHandler;
        static string _cachePathOverride;
        EventHandler<FileSelectionEventArgs> _fileSelectionHandler;
        static Dictionary<string, Action<Cookie>> _pendingGetCookieResultCallbacks = new Dictionary<string, Action<Cookie>>();
        static Dictionary<string, Action<Cookie[]>> _pendingGetCookiesResultCallbacks = new Dictionary<string, Action<Cookie[]>>();
        static Dictionary<string, Action<bool>> _pendingSetCookieResultCallbacks = new Dictionary<string, Action<bool>>();
        const string USER_AGENT_EXCEPTION_MESSAGE = "Unable to set the User-Agent string, because a webview has already been created with the default User-Agent. On Windows and macOS, SetUserAgent() can only be called prior to creating any webviews.";

        protected static string _getCachePath() {

            if (_cachePathOverride != null) {
                return _cachePathOverride;
            }
            // Only Path.Combine(string, string) is available in legacy .NET 3.5.
            return Path.Combine(Application.persistentDataPath, Path.Combine("Vuplex.WebView", "chromium-cache"));
        }

        /// <summary>
        /// The native plugin invokes this method.
        /// </summary>
        void HandleAuthRequested(string host) {

            var handler = _authRequestedHandler;
            if (handler == null) {
                // This shouldn't happen.
                WebViewLogger.LogWarning("The native webview sent an auth request, but no event handler is attached to AuthRequested.");
                WebView_cancelAuth(_nativeWebViewPtr);
                return;
            }
            var eventArgs = new AuthRequestedEventArgs(
                host,
                (username, password) => WebView_continueAuth(_nativeWebViewPtr, username, password),
                () => WebView_cancelAuth(_nativeWebViewPtr)
            );
            handler(this, eventArgs);
        }

        /// <summary>
        /// The native plugin invokes this method.
        /// </summary>
        void HandleDownloadProgressChanged(string serializedMessage) {

            var handler = DownloadProgressChanged;
            if (handler != null) {
                var message = DownloadMessage.FromJson(serializedMessage);
                handler(this, message.ToEventArgs());
            }
        }

        /// <summary>
        /// The native plugin invokes this method.
        /// </summary>
        void HandleFileSelectionRequested(string serializedMessage) {

            var message = FileSelectionMessage.FromJson(serializedMessage);
            Action<string[]> continueCallback = (filePaths) => {
                var serializedFilePaths = JsonUtility.ToJson(new JsonArrayWrapper<string>(filePaths));
                WebView_continueFileSelection(_nativeWebViewPtr, serializedFilePaths);
            };
            Action cancelCallback = () => WebView_cancelFileSelection(_nativeWebViewPtr);
            var eventArgs = new FileSelectionEventArgs(
                message.AcceptFilters,
                message.MultipleAllowed,
                continueCallback,
                cancelCallback
            );
            _fileSelectionHandler(this, eventArgs);
        }

        [AOT.MonoPInvokeCallback(typeof(GetCookieCallback))]
        static void _handleGetCookieResult(string resultCallbackId, string serializedCookie) {

            var callback = _pendingGetCookieResultCallbacks[resultCallbackId];
            _pendingGetCookieResultCallbacks.Remove(resultCallbackId);
            if (callback != null) {
                var cookie = Cookie.FromJson(serializedCookie);
                callback(cookie);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(GetCookieCallback))]
        static void _handleGetCookiesResult(string resultCallbackId, string serializedCookies) {

            var callback = _pendingGetCookiesResultCallbacks[resultCallbackId];
            _pendingGetCookiesResultCallbacks.Remove(resultCallbackId);
            var cookies = Cookie.ArrayFromJson(serializedCookies);
            callback(cookies);
        }

        /// <summary>
        /// The native plugin invokes this method.
        /// </summary>
        void HandlePopup(string message) {

            var handler = PopupRequested;
            if (handler == null) {
                return;
            }
            var components = message.Split(new char[] { ',' }, 2);
            var url = components[0];
            var popupBrowserId = components[1];

            if (popupBrowserId.Length == 0) {
                handler(this, new PopupRequestedEventArgs(url, null));
                return;
            }
            var popupWebView = _instantiate();
            Dispatcher.RunOnMainThread(() => {
                Web.CreateTexture(1, 1, texture => {
                    // Use the same resolution and dimensions as the current webview.
                    popupWebView.SetResolution(_numberOfPixelsPerUnityUnit);
                    popupWebView._initPopup(texture, _width, _height, popupBrowserId);
                    handler(this, new PopupRequestedEventArgs(url, popupWebView as IWebView));
                });
            });
        }

        [AOT.MonoPInvokeCallback(typeof(SetCookieCallback))]
        static void _handleSetCookieResult(string resultCallbackId, bool success) {

            var callback = _pendingSetCookieResultCallbacks[resultCallbackId];
            _pendingSetCookieResultCallbacks.Remove(resultCallbackId);
            if (callback != null) {
                callback(success);
            }
        }

        void _initPopup(Texture2D viewportTexture, float width, float height, string popupId) {

            base.Init(viewportTexture, width, height, null);
            _nativeWebViewPtr = WebView_new(gameObject.name, _nativeWidth, _nativeHeight, popupId);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void _initializePlugin() {

            // The generic GetFunctionPointerForDelegate<T> is unavailable in legacy .NET 3.5.
            var sendMessageFunction = Marshal.GetFunctionPointerForDelegate((UnitySendMessageFunction)_unitySendMessage);
            WebView_setSendMessageFunction(sendMessageFunction);
            WebView_setCookieCallbacks(
                Marshal.GetFunctionPointerForDelegate((GetCookieCallback)_handleGetCookieResult),
                Marshal.GetFunctionPointerForDelegate((GetCookieCallback)_handleGetCookiesResult),
                Marshal.GetFunctionPointerForDelegate((SetCookieCallback)_handleSetCookieResult)
            );
            // cache, cookies, and storage are enabled by default
            _setCachePath(_getCachePath(), "_initializePlugin");
        }

        protected abstract StandaloneWebView _instantiate();

        static void _throwAlreadyInitializedException(string action, string methodName) {

            var message = String.Format("Unable to {0} because a webview has already been created. On Windows and macOS, {1}() can only be called prior to initializing any webviews.", action, methodName);
            throw new InvalidOperationException(message);
        }

        // Partial methods implemented by other 3D WebView packages
        // to provide platform-specific warnings in the editor.
        partial void OnCanGoBack();
        partial void OnCanGoForward();
        partial void OnCaptureScreenshot();
        partial void OnCopy();
        partial void OnCut();
        partial void OnExecuteJavaScript();
        partial void OnGetRawTextureData();
        partial void OnGoBack();
        partial void OnGoForward();
        partial void OnLoadHtml();
        partial void OnLoadUrl(string url);
        partial void OnPaste();

        void _pointerDown(Vector2 point, MouseButton mouseButton, int clickCount) {

            _assertValidState();
            int nativeX = (int) (point.x * _nativeWidth);
            int nativeY = (int) (point.y * _nativeHeight);
            WebView_pointerDown(_nativeWebViewPtr, nativeX, nativeY, (int)mouseButton, clickCount);
        }

        void _pointerUp(Vector2 point, MouseButton mouseButton, int clickCount) {

            _assertValidState();
            int nativeX = (int) (point.x * _nativeWidth);
            int nativeY = (int) (point.y * _nativeHeight);
            WebView_pointerUp(_nativeWebViewPtr, nativeX, nativeY, (int)mouseButton, clickCount);
        }

        static void _setCachePath(string path, string methodName) {

            var cachePath = path == null ? "" : path;
            var success = WebView_setCachePath(cachePath);
            if (!success) {
                _throwAlreadyInitializedException("set the Chromium cache path", methodName);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(UnitySendMessageFunction))]
        static void _unitySendMessage(string gameObjectName, string methodName, string message) {

            Dispatcher.RunOnMainThread(() => {
                var gameObj = GameObject.Find(gameObjectName);
                if (gameObj == null) {
                    WebViewLogger.LogWarningFormat("Unable to deliver a message from the native plugin to a webview GameObject because there is no longer a GameObject named '{0}'. This can sometimes happen directly after destroying a webview. In that case, it is benign and this message can be ignored.", gameObjectName);
                    return;
                }
                gameObj.SendMessage(methodName, message);
            });
        }

        [DllImport(_dllName)]
        static extern void WebView_cancelAuth(IntPtr webViewPtr);

        [DllImport(_dllName)]
        static extern void WebView_cancelFileSelection(IntPtr webViewPtr);

        [DllImport(_dllName)]
        static extern void WebView_continueAuth(IntPtr webViewPtr, string username, string password);

        [DllImport(_dllName)]
        static extern void WebView_continueFileSelection(IntPtr webViewPtr, string serializedFilePaths);

        [DllImport(_dllName)]
        static extern void WebView_copy(IntPtr webViewPtr);

        [DllImport(_dllName)]
        static extern void WebView_cut(IntPtr webViewPtr);

        [DllImport (_dllName)]
        static extern void WebView_dispatchTouchEvent(
            IntPtr webViewPtr,
            int touchID,
            int type,
            float pointX,
            float pointY,
            float radiusX,
            float radiusY,
            float rotationAngle,
            float pressure
        );

        [DllImport(_dllName)]
        static extern bool WebView_enableRemoteDebugging(int portNumber);

        [DllImport(_dllName)]
        static extern void WebView_getCookie(string url, string name, string resultCallbackId);

        [DllImport(_dllName)]
        static extern void WebView_getCookies(string url, string resultCallbackId);

        [DllImport(_dllName)]
        static extern bool WebView_globallySetUserAgentToMobile(bool mobile);

        [DllImport(_dllName)]
        static extern bool WebView_globallySetUserAgent(string userAgent);

        [DllImport(_dllName)]
        static extern void WebView_keyDown(IntPtr webViewPtr, string key, int modifiers);

        [DllImport(_dllName)]
        static extern void WebView_keyUp(IntPtr webViewPtr, string key, int modifiers);

        [DllImport (_dllName)]
        static extern void WebView_movePointer(IntPtr webViewPtr, int x, int y);

        [DllImport(_dllName)]
        static extern IntPtr WebView_new(string gameObjectName, int width, int height, string popupBrowserId);

        [DllImport(_dllName)]
        static extern void WebView_paste(IntPtr webViewPtr);

        [DllImport(_dllName)]
        static extern bool WebView_pluginIsInitialized();

        [DllImport (_dllName)]
        static extern void WebView_pointerDown(IntPtr webViewPtr, int x, int y, int mouseButton, int clickCount);

        [DllImport (_dllName)]
        static extern void WebView_pointerUp(IntPtr webViewPtr, int x, int y, int mouseButton, int clickCount);

        [DllImport(_dllName)]
        static extern void WebView_selectAll(IntPtr webViewPtr);

        [DllImport(_dllName)]
        static extern bool WebView_setAudioAndVideoCaptureEnabled(bool enabled);

        [DllImport(_dllName)]
        static extern void WebView_setAudioMuted(IntPtr webViewPtr, bool muted);

        [DllImport (_dllName)]
        static extern void WebView_setAuthEnabled(IntPtr webViewPtr, bool enabled);

        [DllImport(_dllName)]
        static extern bool WebView_setAutoplayEnabled(bool enabled);

        [DllImport(_dllName)]
        static extern bool WebView_setCachePath(string cachePath);

        [DllImport(_dllName)]
        static extern bool WebView_setCommandLineArguments(string args);

        [DllImport(_dllName)]
        static extern void WebView_setCookie(string serializedCookie, string resultCallbackId);

        [DllImport(_dllName)]
        static extern int WebView_setCookieCallbacks(IntPtr getCookieCallback, IntPtr getCookiesCallback, IntPtr setCookieCallback);

        [DllImport (_dllName)]
        static extern void WebView_setDownloadsEnabled(IntPtr webViewPtr, string downloadsDirectoryPath);

        [DllImport (_dllName)]
        static extern void WebView_setFileSelectionEnabled(IntPtr webViewPtr, bool enabled);

        [DllImport(_dllName)]
        static extern bool WebView_setIgnoreCertificateErrors(bool ignore);

        [DllImport(_dllName)]
        static extern void WebView_setNativeFileDialogEnabled(IntPtr webViewPtr, bool enabled);

        [DllImport(_dllName)]
        static extern void WebView_setPopupMode(IntPtr webViewPtr, int popupMode);

        [DllImport(_dllName)]
        static extern bool WebView_setScreenSharingEnabled(bool enabled);

        [DllImport(_dllName)]
        static extern int WebView_setSendMessageFunction(IntPtr sendMessageFunction);

        [DllImport(_dllName)]
        static extern bool WebView_setTargetFrameRate(uint targetFrameRate);

        [DllImport(_dllName)]
        static extern void WebView_setZoomLevel(IntPtr webViewPtr, float zoomLevel);

        [DllImport(_dllName)]
        static extern void WebView_terminatePlugin();
    }
}
#endif

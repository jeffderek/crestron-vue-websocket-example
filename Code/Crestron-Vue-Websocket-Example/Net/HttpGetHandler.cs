// <copyright file="HttpGetHandler.cs" company="jeffderek">
// The MIT License (MIT)
// Copyright (c) Jeff McAleer
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Text;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace Crestron_Vue_Websocket_Example.Net
{
    /// <summary>
    /// The HttpGetHandler is a subset of the WebUi class that takes care of responding to Http Get requests.
    /// </summary>
    public class HttpGetHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpGetHandler"/> class.
        /// </summary>
        public HttpGetHandler()
        {
        }

        /// <summary>
        /// Webserver Http Get response.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">args.</param>
        public void Http_OnGet(object sender, HttpRequestEventArgs e)
        {
            var request = e.Request;
            var response = e.Response;

            var path = request.Url.GetComponents(UriComponents.Path, UriFormat.UriEscaped);

            if (path == "/" || path == string.Empty)
            {
                path += "index.html";
            }

            byte[] contents;

            if (!e.TryReadFile(path, out contents))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;

                return;
            }

            if (path.EndsWith(".html"))
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/html";
                response.ContentEncoding = Encoding.UTF8;
            }
            else if (path.EndsWith(".js"))
            {
                response.ContentType = "application/javascript";
                response.ContentEncoding = Encoding.UTF8;
            }

            response.ContentLength64 = contents.LongLength;

            response.Close(contents, true);
        }
    }
}

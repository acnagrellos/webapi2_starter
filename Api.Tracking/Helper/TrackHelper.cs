using Api.Tracking.Domain;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;

namespace Api.Tracking.Helper
{
    public class TrackHelper : ITrackHelper
    {
        public virtual EndpointDescription GetLogEndpoint(HttpRequestMessage request, HttpResponseMessage response, string bodyJson)
        {
            var value = response?.Content?.GetType().GetProperty("Value")?.GetValue(response.Content);
            var actionDescriptor = request.GetActionDescriptor();
            var httpCode = (int)response.StatusCode;

            var log = new EndpointDescription
            {
                OriginIP = this.GetClientIpAddress(request),
                UrlRequest = request.RequestUri.ToString(),
                Controller = actionDescriptor.ControllerDescriptor?.ControllerName,
                Method = actionDescriptor.ActionName,
                QueryParameters = this.DictionaryStringJson(request.GetQueryNameValuePairs()),
                BodyMessage = GetBodyLog(actionDescriptor, bodyJson),
                Result = httpCode < 500 ? this.GetStringLog(value) : null,
                HttpResponseStatus = httpCode,
                ErrorMessages = httpCode >= 500 ? this.ObjectStringJson(value) : null,
                Date = DateTime.UtcNow
            };
            return log;
        }

        private string GetBodyLog(HttpActionDescriptor actionDescriptor, string bodyJson)
        {
            var result = bodyJson;
            if (!string.IsNullOrEmpty(bodyJson))
            {
                var parameters = actionDescriptor.GetParameters();

                if (parameters.Any(param => param.ParameterType.IsClass))
                {
                    var type = parameters.First(param => param.ParameterType.IsClass).ParameterType;

                    if (type.GetInterfaces().Contains(typeof(ITrackeable)))
                    {
                        var serializerSettings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        };
                        var body = JsonConvert.DeserializeObject(bodyJson, type, serializerSettings);
                        if (body is ITrackeable)
                        {
                            result = (body as ITrackeable).ToTrackString();
                        }
                    }
                }
            }
            return result;
        }

        private string DictionaryStringJson(IEnumerable<KeyValuePair<string, string>> elements)
        {
            if (elements != null && elements.Count() > 0)
            {
                return string.Join(" , ", elements.Select(item => $"{item.Key}: {this.ObjectStringJson(item.Value)}"));
            }
            return null;
        }

        private string GetStringLog(object target)
        {
            var result = string.Empty;
            if (target != null)
            {
                if (target is ITrackeable)
                {
                    result = ((ITrackeable)target).ToTrackString();
                }
                else
                {
                    var type = target.GetType();
                    if (type.IsPrimitive)
                    {
                        result = target.ToString();
                    }
                    else if (target is IEnumerable)
                    {
                        result = this.ObjectTrackString((target as IEnumerable));
                    }
                    else
                    {
                        var properties =
                            from property in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            select new
                            {
                                Name = property.Name,
                                Value = property.GetValue(target, null)
                            };

                        var propertiesStr = string.Join("\r\n", properties.Select(prop => $"\t{prop.Name} = {this.ObjectTrackString(prop.Value)}"));

                        result = $"{target.GetType().Name} = {{\r\n{propertiesStr}\r\n}}";
                    }
                }
            }
            return result;
        }

        private string ObjectTrackString(object value)
        {
            var result = string.Empty;
            if (value != null)
            {
                if (value is ITrackeable)
                {
                    result = ((ITrackeable)value).ToTrackString();
                }
                else
                {
                    var type = value.GetType();
                    if (value is IEnumerable)
                    {
                        var count = (value as IEnumerable).OfType<object>().Count();
                        var objectType = string.Empty;
                        var genericArgument = type.GetGenericArguments();
                        if (genericArgument.Count() > 0) objectType = genericArgument[0].Name;
                        result = $"IEnumerable[{objectType}] ({count})";
                    }
                    else if (type.IsPrimitive)
                    {
                        result = value.ToString();
                    }
                    else
                    {
                        result = $"{type.Name}";
                    }
                }
            }
            else
            {
                result = "null";
            }
            return result;
        }

        private string ObjectStringJson(object value)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(value, Formatting.Indented, serializerSettings);
        }

        private string GetClientIpAddress(HttpRequestMessage request)
        {
            string HttpContext = "MS_HttpContext";
            string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
            string OwinContext = "MS_OwinContext";

            // Web-hosting. Needs reference to System.Web.dll
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            // Self-hosting. Needs reference to System.ServiceModel.dll. 
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            // Self-hosting using Owin. Needs reference to Microsoft.Owin.dll. 
            if (request.Properties.ContainsKey(OwinContext))
            {
                dynamic owinContext = request.Properties[OwinContext];
                if (owinContext != null)
                {
                    return owinContext.Request.RemoteIpAddress;
                }
            }

            return null;
        }
    }
}

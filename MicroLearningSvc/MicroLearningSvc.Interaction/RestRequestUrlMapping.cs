using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MicroLearningSvc.Interaction
{
    class RestRequestIrlMappingParameterInfo
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public int Index { get; private set; }

        public RestRequestIrlMappingParameterInfo(string name, Type type, int index)
        {
            this.Name = name;
            this.Type = type;
            this.Index = index;
        }

        static readonly Type[] _numericTypes = new[]{
            typeof(byte),typeof(sbyte),
            typeof(short),typeof(ushort),
            typeof(int),typeof(uint),
            typeof(long),typeof(ulong),
        };

        public string GetRegexStr()
        {
            if (_numericTypes.Contains(this.Type))
                return "[0-9]+";

            if (this.Type == typeof(bool))
                return "(true)|(false)|0|1|(yes)|(no)";

            if (this.Type == typeof(string))
                return @"[^\/\\\=\&\?]+";

            if (this.Type == typeof(Guid))
                return @"[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?";

            throw new InvalidOperationException();
        }

        static readonly Type[] _nonNumTypes = new[]{
            typeof(string), typeof(bool), typeof(Guid)
        };

        public static bool IsGetParameterType(Type type)
        {
            return _numericTypes.Contains(type) || _nonNumTypes.Contains(type);
        }
    }

    class RestRequestUrlMapping
    {
        public string UrlFragmentTemplate { get; private set; }

        public int ParamsCount { get; private set; }
        public ReadOnlyCollection<RestRequestIrlMappingParameterInfo> UrlParametersInfo { get; private set; }

        public int? ContextParameterIndex { get; private set; }
        public RestRequestIrlMappingParameterInfo BodyParameterInfo { get; private set; }

        public Regex UriFragmentRegex { get; private set; }

        // readonly Delegate _handler;

        public RestRequestUrlMapping(string uriFragmentTemplate, MethodBase method)
        {
            this.UrlFragmentTemplate = uriFragmentTemplate;

            // TODO: ?????
            //if (uriFragmentTemplate.Contains('/') || uriFragmentTemplate.Contains('\\'))
            //    throw new ArgumentException("uriFragmentTemplate");

            var paramsInfo = method.GetParameters().Select((p, i) => new RestRequestIrlMappingParameterInfo(p.Name, p.ParameterType, i)).ToList();

            int getParamsCount;
            this.UriFragmentRegex = this.MakeRegex(uriFragmentTemplate, paramsInfo, out getParamsCount);

            this.ParamsCount = paramsInfo.Count;

            //var contextParamIndex = paramsInfo.IndexOf(p => typeof(IRestOperationContext).IsAssignableFrom(p.Type));
            var bodyParamIndex = paramsInfo.IndexOf(p => !RestRequestIrlMappingParameterInfo.IsGetParameterType(p.Type) || !uriFragmentTemplate.Contains($"{{{p.Name}}}"));

            //if (contextParamIndex >= 0)
            //{
            //    this.ContextParameterIndex = contextParamIndex;
            //    paramsInfo.RemoveAt(contextParamIndex);
            //}

            if (bodyParamIndex >= 0)
            {
                this.BodyParameterInfo = paramsInfo[bodyParamIndex];
                paramsInfo.RemoveAt(bodyParamIndex);
            }

            //if (paramsInfo.Any(p => !RestRequestIrlMappingParameterInfo.IsGetParameterType(p.Type)))
            //    throw new ArgumentException("There may be only one complex argument from body!");

            this.UrlParametersInfo = new ReadOnlyCollection<RestRequestIrlMappingParameterInfo>(paramsInfo);
        }

        private Regex MakeRegex(string uriFragmentTemplate, IEnumerable<RestRequestIrlMappingParameterInfo> paramsInfo, out int getParamsCount)
        {
            var matches = Regex.Matches(uriFragmentTemplate, @"\{[0-9\w]+\}");

            var names = new string[matches.Count];
            var regexStr = new StringBuilder();
            regexStr.Append("^");

            var index = 0;
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                regexStr.Append(Regex.Escape(uriFragmentTemplate.Substring(index, match.Index - index)));

                var valueName = match.Value.Substring(1, match.Length - 2);
                var valueParamInfo = paramsInfo.First(p => p.Name == valueName);
                var valueFormat = valueParamInfo.GetRegexStr();
                regexStr.Append($"(?<{valueName}>{valueFormat})");

                index = match.Index + match.Length;
            }

            regexStr.Append(Regex.Escape(uriFragmentTemplate.Substring(index)));
            regexStr.Append("$");

            getParamsCount = matches.Count;
            return new Regex(regexStr.ToString());
        }

        public void FillParameters(IEnumerable<KeyValuePair<string,object>> parameters, object[] args)
        {
            foreach (var item in this.UrlParametersInfo)
            {
                args[item.Index] = parameters.First(p => p.Key == item.Name).Value;
            }

            //if (this.BodyParameterInfo != null)
            //    args[this.BodyParameterInfo.Index] = body;

            //var result = _handler.DynamicInvoke(args);
            //return result;
        }
    }
}

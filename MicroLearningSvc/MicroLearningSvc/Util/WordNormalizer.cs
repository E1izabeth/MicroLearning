using LanguageDetection;
using MicroLearningSvc.Impl;
using NTextCat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ufal.UDPipe;
using Ufal.UDPipe.CSharp.Unofficial.Helpers;

namespace MicroLearningSvc.Util
{
    class WordNormalizer : IWordNormalizer
    {
        readonly Dictionary<string, Model> _modelByLang = new Dictionary<string, Model>();
        readonly Dictionary<string, FileInfo> _modelFileByLang = new Dictionary<string, FileInfo>();
        readonly string _modelsLocation;

        readonly NaiveBayesLanguageIdentifier _naiveBayesLanguageIdentifier;
        readonly RankedLanguageIdentifier _rankedLanguageIdentifier;

        readonly object _lock = new object();


        // https://lindat.mff.cuni.cz/repository/xmlui/bitstream/handle/11234/1-3131/udpipe-ud-2.5-191206.zip?sequence=1&isAllowed=ys
        public WordNormalizer(string langModelsLocation)
        {
            _modelsLocation = langModelsLocation;
         
            var naiveBayesLanguageIdentifierFactory = new NaiveBayesLanguageIdentifierFactory();
            _naiveBayesLanguageIdentifier = naiveBayesLanguageIdentifierFactory.Load(new MemoryStream(Properties.Resources.NTextCatCore14ProfileXml));

            var rankedLanguageIdentifierFactory = new RankedLanguageIdentifierFactory();
            _rankedLanguageIdentifier = rankedLanguageIdentifierFactory.Load(new MemoryStream(Properties.Resources.NTextCatCore14ProfileXml));
        }

        private string DetectWordLanguage(string word)
        {
            var lang1 = _naiveBayesLanguageIdentifier.Identify(word).FirstOrDefault()?.Item1?.Iso639_3;
            var lang2 = _naiveBayesLanguageIdentifier.Identify(word).FirstOrDefault()?.Item1?.Iso639_3;

            if (lang1 != null && lang2 != null && lang1 == lang2)
            {
                return lang1;
            }
            else
            {
                var detector = new LanguageDetector();
                detector.AddAllLanguages();
                var langName = detector.Detect(word);

                if (langName == lang1)
                    return lang1;
                else if (langName == lang2)
                    return lang2;
                else
                    return langName;
            }
        }

        private Model GetModelForWord(string word)
        {
            if (_modelFileByLang.Count == 0)
            {
                var modelsDir = new DirectoryInfo(_modelsLocation);
                foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                                   .Select(c => new
                                                   {
                                                       isoName = c.ThreeLetterISOLanguageName,
                                                       englishName = c.EnglishName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
                                                   })
                                                   .DistinctBy(c => c.englishName)
                                                   .DistinctBy(c => c.isoName))
                {
                    // Console.WriteLine(culture);
                    
                    var modelFile = modelsDir.GetFiles(culture.englishName + "*.udpipe")
                                             .OrderByDescending(f => f.Length)
                                             .FirstOrDefault();
                    if (modelFile != null)
                        _modelFileByLang.Add(culture.isoName, modelFile);
                }
            }

            var langName = this.DetectWordLanguage(word);

            if (!_modelByLang.TryGetValue(langName, out var langModel))
            {
                if (_modelFileByLang.TryGetValue(langName, out var modelFile))
                {
                    langModel = Model.load(modelFile.FullName);

                    if (langModel == null)
                        throw new ApplicationException("Failed to load model from file " + modelFile.FullName);
                    
                    _modelByLang.Add(langName, langModel);
                }
                else
                {
                    throw new ApplicationException($"No model to normalize word '{word}' of {langName} language");
                }
            }

            return langModel;
        }

        public string NormalizeWord(string word)
        {
            //{
            //    var mystem = new YandexMystem.Wrapper.Mysteam();
            //    mys
            //    var wordInfo = mystem.GetWords(word).First();
            //}
            lock (_lock)
            {
                var model = this.GetModelForWord(word);

                Pipeline pipeline = new Pipeline(model, "tokenize", Pipeline.DEFAULT, Pipeline.DEFAULT, "conllu");

                using (ProcessingError error = new ProcessingError())
                {
                    var wordInfo = pipeline.process(word, error);
                    if (error.occurred())
                        throw new Exception($"Error occured {error.message}");

                    var conlluParser = new ConlluParser();
                    var normalWord = conlluParser.ToLemText(wordInfo);
                    return normalWord;
                }
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var item in _modelByLang.Values)
                    item.SafeDispose();

                _modelByLang.Clear();
            }
        }
    }
}

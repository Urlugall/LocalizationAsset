using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Events;

namespace Language
{
    public class Localization : MonoBehaviour
    {
        [SerializeField] private string fileName = "localization.csv";
        [SerializeField] private Language defaultLanguage = Language.Eng;
        
        public static Localization Instance { get; private set; } // static instance
        
        private readonly List<(string code, string[] translations)> _translationData = new ();
        
        private Language _userLanguage;
        public Language UserLanguage
        {
            get => _userLanguage;
            set => SetLanguage(value);
        }

        private void Awake()
        {
            Instance = this;
            
            ReadTranslationData();
            LoadLanguagePreference();
            
            DontDestroyOnLoad(this);
        }

        #region UserOptions

        public string Localize(string code)
        {
            var translation = _translationData.FirstOrDefault(t => t.code == code);
        
            if (!translation.translations.Any())
            {
                Debug.LogWarning($"No translation '{_userLanguage}' available for code: {code}");
                return translation.translations[(int)defaultLanguage];
            }

            return translation.translations[(int)_userLanguage];
        }
        
        public static event UnityAction OnLanguageChanged;
        public void SetLanguage(Language newLanguage)
        {
            _userLanguage = newLanguage;
            SaveLanguagePreference();
            
            // Notify all listeners about the language change
            OnLanguageChanged?.Invoke();
            Debug.Log($"Language was set to {newLanguage}");
        }

        #endregion

        
        
        private void ReadTranslationData()
        {
            var path = Path.Combine(Application.streamingAssetsPath, fileName);

            using var reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                var data = reader.ReadLine()!.Split(',');
                _translationData.Add((data[0], data[1..]));
            }
        }
        
        
        
        #region PlayerPrefs Language Persistence

        private const string LanguagePrefKey = "UserLanguage";

        private void SaveLanguagePreference()
        {
            PlayerPrefs.SetInt(LanguagePrefKey, (int)_userLanguage);
            PlayerPrefs.Save();
        }

        private void LoadLanguagePreference()
        {
            SetLanguage(PlayerPrefs.HasKey(LanguagePrefKey) ? 
                (Language)PlayerPrefs.GetInt(LanguagePrefKey) : 
                defaultLanguage);
        }

        #endregion

        
        
        #region Localization File

        public void CheckLocalizationFile()
        {
            var path = Path.Combine(Application.streamingAssetsPath, fileName);

            if (!File.Exists(path))
            {
                Debug.Log("File does not exist. Creating new file...");
                CreateLocalizationFile(path);
                ReadTranslationData();
            }
        }

        private void CreateLocalizationFile(string path)
        {
            string[] headers = { "code", "Eng", "Deu" };
            string[] data = {
                "greeting,Hello,Hallo",
                "farewell,Goodbye,Auf Wiedersehen"
            };

            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());

            try
            {
                File.WriteAllLines(path, new[] { string.Join(",", headers) }.Concat(data));
                Debug.Log("Localization file created at: " + path);
            }
            catch (IOException ex)
            {
                Debug.LogError("Error creating localization file: " + ex.Message);
            }
        }

        #endregion
        

        public enum Language
        {
            [Tooltip("English")]
            Eng,
            [Tooltip("German")]
            Deu,
            [Tooltip("French")]
            Fra,
            [Tooltip("Russian")]
            Rus,
            [Tooltip("Spanish")]
            Spa,
            [Tooltip("Ukrainian")]
            Ukr,
            [Tooltip("Hindi")]
            Hin
        }
    }
}
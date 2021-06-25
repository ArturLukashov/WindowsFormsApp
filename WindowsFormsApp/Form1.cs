using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Microsoft.VisualC.StlClr;

namespace WindowsFormsApp
{

  
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public OpenFileDialog openFile = new OpenFileDialog();
        public string json_text;
        private void Choose_File_Click(object sender, EventArgs e)
        {
            if (ChooseFile() != null)
            {
                Read_from_file.Enabled = true; 
            }            
        }
        private Dictionary<string, int> Sort(Dictionary<string,int> pairs)
        {
            var sortedDict = from entry in pairs orderby entry.Value descending select entry;
            return sortedDict.ToDictionary<KeyValuePair<string, int>, string, int>(pair => pair.Key, pair => pair.Value);
        }

        private string ChooseFile()
        {            
            openFile.Filter = "txt files (*.txt)|*.txt;";         
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFile.SafeFileName;
                return openFile.FileName;
            }
            else
            {
                return null; 
            }                       
           
        }
        public async void ForAnaliz()
        {
            await Task.Run(() => Analiz());

        }
        public void Proccess_Message()
        {
            Анализ_файла analiz = new Анализ_файла();
            analiz.ShowDialog();
        }
        public void Analiz()
        {            
            Thread myThread = new Thread(Proccess_Message); 
            myThread.Start(); 
            try
            {
                Test test = new Test();
                test.filename = openFile.SafeFileName;
                test.allfields.Add("wordsCount", 0);
                char[] separators = new char[] { ' ', '\r', '\n' };
                StreamReader stream = new StreamReader(openFile.FileName);
                string[] every_line = stream.ReadToEnd().Split('\n');
                for (int i = 0; i < every_line.Length; i++)
                {
                    for (int j = 0; j < every_line[i].Length; j++)
                    {
                        test.Count_of_letters(every_line[i][j]);
                        test.Count_of_punctuation_marks(every_line[i][j]);
                        test.Count_of_digits(every_line[i][j]);

                    }
                    string[] line = every_line[i].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    for (int k = 0; k < line.Length; k++)
                    {
                        test.Count_of_numbers(line[k]);
                        test.Count_of_each_word(line[k]);
                    }
                    test.allfields["wordsCount"] += line.Length;
                }
                test.allfields.Add("linesCount", every_line.Length);       
                test.Longest_word();
                test.Count_words_with_hyphen();
                test.allfields.Add("filesize",Convert.ToInt32(new System.IO.FileInfo(openFile.FileName).Length));
                test.words = Sort(test.words);
                test.allfields = Sort(test.allfields);
                var sortedDict = from entry in test.letters orderby entry.Value descending select entry;
                test.letters = sortedDict.ToDictionary<KeyValuePair<char, int>, char, int>(pair => pair.Key, pair => pair.Value);
                json_text = JsonConvert.SerializeObject(test, Formatting.Indented);                
                File.WriteAllText("serialized.json", json_text);
                myThread.Abort();          
                MessageBox.Show("Файл с результатами успешно сохранён {serialized.json}", "Оповещение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start("serialized.json");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

              
        }

        private void Read_from_file_Click(object sender, EventArgs e)
        {
            ForAnaliz();          
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
    }
    public class Test 
    {          
        public string filename { get; set; }
        public Dictionary<string, int> allfields { get; set; }
        public string longestWord { get; set; }
        public Dictionary<char, int> letters { get; set; }
        public Dictionary<string, int> words { get; set; }
         
        public Test()
        {                      
            letters = new Dictionary<char, int>();
            words = new Dictionary<string, int>();
            allfields = new Dictionary<string, int>
            {
                ["lettersCount"] = 0,
                ["punctuation"] = 0,
                ["digitsCount"] = 0,
                ["numbersCount"] = 0,
                ["wordsWithHyphen"] = 0
            };
        }

        //количество букв всего; количества каждой буквы
        public void Count_of_letters(char letter)
        {
            int lettersCount = 0;
            if (char.IsLetter(letter) == true)
            {                              
                if (letters.ContainsKey(char.ToLower(letter)) == false)
                {
                    letters.Add(char.ToLower(letter), 1);
                }
                else
                {
                    letters[char.ToLower(letter)] += 1;
                }
                lettersCount++;
            }
            allfields["lettersCount"]+=lettersCount;
        }
        //Количество знаков препинания
        public void Count_of_punctuation_marks(char symbol)
        {
            int punctuation = 0;
            if (char.IsPunctuation(symbol) == true)
            {
                punctuation++;
            }
            allfields["punctuation"] += punctuation;
        }
        //Количество цифр
        public void Count_of_digits(char symbol)
        {
            int digitsCount = 0;
            if (char.IsNumber(symbol) == true)
            {
                digitsCount++;
            }
            allfields["digitsCount"]+= digitsCount;
        }
        //Количество чисел
        public void Count_of_numbers(string word)
        {
            int numbersCount = 0; 
            if (Regex.IsMatch(word, @"\d+"))
            {
                numbersCount++;
            }
            allfields["numbersCount"] += numbersCount;
        }
        //Количество каждого слова
        public void Count_of_each_word(string key)
        {
            if (words.ContainsKey(key) == false)
            {
                words.Add(key, 1);
            }
            else
            {
                words[key] += 1;
            }
        }
        //Самое длинное слово
        public void Longest_word()
        {
            longestWord = words.OrderBy(x => x.Key).First().Key;
            int maxLength = longestWord.Length;
            foreach (var x in words)
            {
                if (x.Key.Length > maxLength)
                {
                    maxLength = x.Key.Length;
                    longestWord = x.Key;
                }              
            }
        }
        //Количество слов с дефисом
        public void Count_words_with_hyphen()
        {
            int wordsWithHyphen = 0; 
            foreach (var x in words)
            {               
                if (x.Key.Contains('-') && x.Key.Length>1 == true)
                {
                    wordsWithHyphen++;
                }
            }
            allfields["wordsWithHyphen"]+= wordsWithHyphen;
        }
        
    }
}


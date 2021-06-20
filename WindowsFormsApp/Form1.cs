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


namespace WindowsFormsApp
{

  
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public OpenFileDialog openFile = new OpenFileDialog();
        private void Choose_File_Click(object sender, EventArgs e)
        {
            if (ChooseFile() != null)
            {
                Read_from_file.Enabled = true; 
            }            
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

        private void Read_from_file_Click(object sender, EventArgs e)
        {
            try
            {
                Test test = new Test();
                test.filename = openFile.SafeFileName;
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
                    test.wordsCount += line.Length;
                }
                test.linesCount = every_line.Length;

                test.Longest_word();

                test.Count_words_with_hyphen();

                test.filesize = Convert.ToInt32(new System.IO.FileInfo(openFile.FileName).Length);

                var sortedDict = from entry in test.words orderby entry.Value ascending select entry;
                test.words = sortedDict.ToDictionary<KeyValuePair<string, int>, string, int>(pair => pair.Key, pair => pair.Value);

                string json = JsonConvert.SerializeObject(test, Formatting.Indented);
             
                File.WriteAllText("results.json", json);

                richTextBox1.Text = json;

                MessageBox.Show("Файл с результатами успешно сохранён {results.json}", "Оповещение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

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
        //Имя файла
        public string filename { get; set; }
        //Размер файла
        public int filesize { get; set; }
        //Количество букв всего
        public int lettersCount { get; set; }
        //Количество каждой буквы
        public SortedDictionary<char, int> letter_how_much { get; set; }
        //Количество знаков препинания
        public int punctuation { get; set; }
        //Количество цифр
        public int digitsCount { get; set; }
        //Количество чисел
        public int numbersCount { get; set; }
        //Количество всех слов
        public int wordsCount { get; set; }
        //Количество каждого слова
        public Dictionary<string, int> words { get; set; }
        //Самое длинное слово
        public string longestWord { get; set; }
        //Количество строк
        public int linesCount { get; set; }
        //Количество слов с дефисом
        public int wordsWithHyphen { get; set; }      

        public Test()
        {                      
            letter_how_much = new SortedDictionary<char, int>();
            words = new Dictionary<string, int>();
        }

        //количество букв всего; количества каждой буквы
        public void Count_of_letters(char letter)
        {            
            if (char.IsLetter(letter) == true)
            {                              
                if (letter_how_much.ContainsKey(char.ToLower(letter)) == false)
                {
                    letter_how_much.Add(char.ToLower(letter), 1);
                }
                else
                {
                    letter_how_much[char.ToLower(letter)] += 1;
                }
                lettersCount++;
            }
        }
        //Количество знаков препинания
        public void Count_of_punctuation_marks(char symbol)
        {
            if (char.IsPunctuation(symbol) == true)
            {
                punctuation++;
            }
        }
        //Количество цифр
        public void Count_of_digits(char symbol)
        {
            if (char.IsNumber(symbol) == true)
            {
                digitsCount++;
            }
        }
        //Количество чисел
        public void Count_of_numbers(string word)
        {
            if (Regex.IsMatch(word, @"\d+"))
            {
                numbersCount++;
            }
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
            foreach (var x in words)
            {               
                if (x.Key.Contains('-') == true)
                {
                    wordsWithHyphen++;
                }
            }
        }
        
    }
}


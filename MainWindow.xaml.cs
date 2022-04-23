using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ITYPE_Game
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
/**
         Attributes*/
		
	    private ArrayList textList;	    
	    //private TimeSpan levelRemainedTime;
	    private string currentText = "ITYPE_Game";
	    private int currentIndex;
	    private int currentLevel=1;
	    private int totalKeyPressCount;
	    private int validKeyPresssCount;
		private int score ;
        private int scoreStep = 100;
	    private int lifePoints = 100;
	    private int currentPath = 1;
	    private double opacity ;
		private double time;//time used to type all words
		private int wordCount;
        private int validWordCount;
	    private bool trainingMode;
        private bool gamePaused = false;
        private bool gameOn = false;
        //Storyboards
	    private readonly Storyboard textStoryboard;
        /*
         Construcor
         */
		public MainWindow()
		{
		    this.InitializeComponent();
            textStoryboard = this.Resources["textStoryboard"] as Storyboard;			
					
		}
        /// <summary>
        /// this method load and store words from a text file to the variable textList
        /// </summary>
	    private void loadTextFromFile(string fileName)
	    {
         
	        try
	        {
                string[] textArray = File.ReadAllLines(@"data/dictionaries/"+fileName+"list.dat");
                textList= new ArrayList(textArray);
	            textList.Remove("");
	        }
	        catch (Exception e)
	        {
	            MessageBox.Show(e.Message);
                Close();
	        }
	    }
        /// <summary>
        /// uses to be able to move the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
	    private void Move(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        /// <summary>
        /// Close the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Start a new game or pause or resume the current game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartNewGame(object sender, RoutedEventArgs e)
        {
            (Resources["FromMenuToOptions"] as Storyboard).Begin(this);                
                currentLevel = Properties.Settings.Default.set_start_level;                
            }
        
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            

            //animating the Virtual keyBoard key press
            if(trainingMode) vkb.clickButton(e.Key);
            //  (Resources["Click"] as Storyboard).Begin(this);
			if(e.Key == Key.Space)
			{
				if(!textStoryboard.GetIsPaused(this))
				{
					textStoryboard.Pause(this);
					e.Handled= true;
                    (Resources["Pause"] as Storyboard).Begin(this);
					gamePaused= true;
					
				}else
				{
                    (Resources["Resume"] as Storyboard).Begin(this);
					textStoryboard.Resume(this);
					
				}
                return;
			}else
			{
				if(gamePaused) {				
				e.Handled= true;
                
			}
				
			}
			
            if(e.Key.ToString().ToLower()==currentText[currentIndex].ToString().ToLower())
            {				
                currentIndex++;//next cahr in the word/text
                validKeyPresssCount++;                
                score += scoreStep*currentLevel;
                if (!trainingMode)
                {
                    opacity += 0.04f;
                    if (opacity > 0.95f)
                    {
                        opacity = 0.0f;
                        currentPath++;
                    }
                    if (currentPath == 5)
                    {
                        currentPath = 1;
                        currentLevel++;
                        (Resources["levelUp"] as Storyboard).Begin(this);
                        path1.Opacity = path2.Opacity = path3.Opacity = path4.Opacity = 0;
                        textStoryboard.SpeedRatio = 1 + currentLevel/10.0f;
                    }
                    if (currentPath == 1) path1.Opacity = opacity;
                    else if (currentPath == 2) path2.Opacity = opacity;
                    else if (currentPath == 3) path3.Opacity = opacity;
                    else if (currentPath == 4) path4.Opacity = opacity;
                }
                if(currentIndex==currentText.Length)// word completed
                {
					
                    currentText = textList[new Random().Next()%textList.Count].ToString();// load new word
                    currentIndex = 0;
					wordCount++;
                    validWordCount++;                    
                    (Resources["newText"] as Storyboard).Begin(this);
                    
                    if(!trainingMode)
                    {
                        TimeSpan? ts = textStoryboard.GetCurrentTime(this);
                        if (ts.HasValue)
                        {
                            time += ts.Value.TotalMilliseconds;
                        }
                        textStoryboard.Stop();
                        textStoryboard.Begin(this, true);
                    }
                    
                    
                }
                
            }
            else
            {
                lifePoints -=1;

                if(lifePoints <0) // GAME OVER
                {
                    this.PreviewKeyDown -= Window_PreviewKeyDown;
                    tbScoreOver.Text = score.ToString();
                    (Resources["gameOver"] as Storyboard).Begin(this);
                }
            }
            totalKeyPressCount++;
            UpdateUI();
            e.Handled = true;
        }
        /// <summary>
        /// update the textBolc text according to the remained letters of the text
        /// </summary>
        private void UpdateUI()
        {
            
            tbDisplyedText.Text = currentText.Substring(currentIndex);
			tbDisplyedText_Copy.Text= currentText.Substring(0,currentIndex);
            if(time>0)
			tbSpeedAverage.Text= Convert.ToInt32(60*(validWordCount/(time/1000)))+"words/mn";
            tbScore.Text =  score.ToString();
            tbLifePoints.Text = lifePoints.ToString();
            tbMissedPresss.Text = (totalKeyPressCount - validKeyPresssCount).ToString();
            tbTotalPress.Text = totalKeyPressCount.ToString();
            if(totalKeyPressCount>0)
            tbSuccessAverage.Text = Convert.ToInt32((100.0f * validKeyPresssCount) / totalKeyPressCount)+"%";
            tbLevel.Text = currentLevel.ToString();
            tbTotalWords.Text = wordCount.ToString();
            tbMissedWords.Text = (wordCount - validWordCount).ToString();


        }
        public void loadNewWord()
        {
			if(textStoryboard.GetCurrentState(this) == ClockState.Active) return;
            wordCount++;
            lifePoints -= currentText.Length;
            currentText = textList[new Random().Next() % textList.Count].ToString();
            currentIndex = 0;
            textStoryboard.Begin(this,true);
            UpdateUI();
        }
        private void btnRating_Click(object sender, RoutedEventArgs e)
        {
            string[] list = Properties.Settings.Default.set_top1.Split(',');            
            SortedDictionary<int,string> rating =new SortedDictionary<int, string>();
            top1.Text = Properties.Settings.Default.set_top1.Split(',')[0] ;
            top2.Text = Properties.Settings.Default.set_top2.Split(',')[0] ;
            top3.Text = Properties.Settings.Default.set_top3.Split(',')[0] ;
            top4.Text = Properties.Settings.Default.set_top4.Split(',')[0];
            top5.Text = Properties.Settings.Default.set_top5.Split(',')[0];
            top1score.Text = Properties.Settings.Default.set_top1.Split(',')[1];
            top2score.Text = Properties.Settings.Default.set_top2.Split(',')[1];
            top3score.Text = Properties.Settings.Default.set_top3.Split(',')[1];
            top4score.Text = Properties.Settings.Default.set_top4.Split(',')[1];
            top5score.Text = Properties.Settings.Default.set_top5.Split(',')[1];            
        }

        


        private void btnDone_Click(object sender, RoutedEventArgs e)
        {

            var sb = Resources["FromOptionsToMenu"] as Storyboard;
            if (sb != null) sb.Begin(this);
            Properties.Settings.Default.set_start_level = Convert.ToInt32(slider.Value);
            Properties.Settings.Default.Save();
            //
            InitializeVariables();
            
            if(tgbMode.IsChecked.GetValueOrDefault())
            {
                trainingMode = true;
                (Resources["showVkb"] as Storyboard).Begin(this);                                
            }
            else
            {
                trainingMode = false;
				(Resources["hideVkb"] as Storyboard).Begin(this);                                
                textStoryboard.Begin(this,true);
            }
        }

	    private void InitializeVariables()
	    {
	        var dictionary = !tgbLang.IsChecked.GetValueOrDefault() ? "English" : "French";

            loadTextFromFile(dictionary);
            currentText = textList[new Random().Next() % textList.Count].ToString();
	        currentLevel = Properties.Settings.Default.set_start_level;
	        currentIndex = 0;
	        currentPath = 1;
	        opacity = 0;
	        score = 0;
			path1.Opacity=path2.Opacity=path3.Opacity=path4.Opacity=0;
	        totalKeyPressCount = validKeyPresssCount = wordCount = validWordCount = 0;
	        lifePoints = 100 ;
	        time = 0 ;
			textStoryboard.SpeedRatio = 1 + currentLevel/10.0f;
            gameOn = false ;
	    }
		
		private void About(object sender, RoutedEventArgs e)
		{
			
			
		}

        private void GameOverOk(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += Window_PreviewKeyDown;
            textStoryboard.Stop(this);
            List<int> list = new List<int>();
            if (Properties.Settings.Default.set_top1.Split(',')[1] != "-") list.Add(int.Parse(Properties.Settings.Default.set_top1.Split(',')[1]));
            if (Properties.Settings.Default.set_top2.Split(',')[1] != "-") list.Add(int.Parse(Properties.Settings.Default.set_top2.Split(',')[1]));
            if (Properties.Settings.Default.set_top3.Split(',')[1] != "-") list.Add(int.Parse(Properties.Settings.Default.set_top3.Split(',')[1]));
            if (Properties.Settings.Default.set_top4.Split(',')[1] != "-") list.Add(int.Parse(Properties.Settings.Default.set_top4.Split(',')[1]));
            if (Properties.Settings.Default.set_top5.Split(',')[1] != "-") list.Add(int.Parse(Properties.Settings.Default.set_top5.Split(',')[1]));

            int pos =0;
            while (pos < list.Count && score < list[pos])
            {
                pos++;
            }
            if(pos<5)
            {
                if (pos == 0)
                {
                    Properties.Settings.Default.set_top5 = Properties.Settings.Default.set_top4;
                    Properties.Settings.Default.set_top4 = Properties.Settings.Default.set_top3;
                    Properties.Settings.Default.set_top3 = Properties.Settings.Default.set_top2;
                    Properties.Settings.Default.set_top2 = Properties.Settings.Default.set_top1;
                    Properties.Settings.Default.set_top1 = tbFinalScore.Text + "," + score;
                }

                if (pos == 1)
                {
                    Properties.Settings.Default.set_top5 = Properties.Settings.Default.set_top4;
                    Properties.Settings.Default.set_top4 = Properties.Settings.Default.set_top3;
                    Properties.Settings.Default.set_top3 = Properties.Settings.Default.set_top2;                    
                    Properties.Settings.Default.set_top2 = tbFinalScore.Text + "," + score;
                }

                if (pos == 2)
                {
                    Properties.Settings.Default.set_top5 = Properties.Settings.Default.set_top4;
                    Properties.Settings.Default.set_top4 = Properties.Settings.Default.set_top3;                    
                    Properties.Settings.Default.set_top3 = tbFinalScore.Text + "," + score;
                }

                if (pos == 3)
                {
                    Properties.Settings.Default.set_top5 = Properties.Settings.Default.set_top4;
                    Properties.Settings.Default.set_top4 = tbFinalScore.Text + "," + score;
                }

                if (pos == 4)
                {                    
                    Properties.Settings.Default.set_top5 = tbFinalScore.Text + "," + score;
                }

                Properties.Settings.Default.Save();
            }
            
            
        }
			
	}


}
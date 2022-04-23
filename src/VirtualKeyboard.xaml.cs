using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

namespace ITYPE_Game
{
	/// <summary>
	/// Logique d'interaction pour VirtualKeyboard.xaml
	/// </summary>
	public partial class VirtualKeyboard : UserControl
	{
		public VirtualKeyboard()
		{
			this.InitializeComponent();
		}
        /// <summary>
        /// Function used to animate a click on a virtualKeyBord button
        /// </summary>
        /// <param name="key"> the pressed key from the keyboard</param>
        public void clickButton(Key key)
        {

            if (btnOnnOff.IsChecked == false) return;
            Button btn=null;
            
            foreach (var element in keys.Children)
            {
               if(element is Button)
               {

                   //MessageBox.Show(key.ToString() + ((Button)element).Tag.ToString());
                   if(((Button)element).Tag.ToString().ToLower()==key.ToString().ToLower())
                   {
                       btn = element as Button;
                       
                       break;
                   }
               }
            }
            if (btn != null) 
                if(btn.IsEnabled== false) return;

            if (btn != null)
            {
                var peer =new ButtonAutomationPeer(btn);
                var invokeProv =peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                if (invokeProv != null) invokeProv.Invoke();
                btn.Focus();
            }
        }

        private void btnOnnOff_Click(object sender, RoutedEventArgs e)
        {
            if(btnOnnOff.IsChecked== true)
            pathVisibility.Visibility = Visibility.Hidden;
            else
            {
                pathVisibility.Visibility = Visibility.Visible;
                
            }
            
        }

        private void tbAzQwSwich_Click(object sender, RoutedEventArgs e)
        {
            if(tbAzQwSwich.IsChecked== false)// azerty to qwerty
            {
                a.Content = "q";
                a.Tag = "Q";
                q.Content = "a";
                q.Tag = "A";
                z.Content = "w";
                z.Tag = "W";
                w.Content = "z";
                w.Tag = "Z";
                m.Content = ",";
                m.Tag = "OemComma";
                OemComma.Content = "m";
                OemComma.Tag = "M";
            }
            else// qwerty to azerty
            {
                q.Content = "q";
                q.Tag = "Q";
                a.Content = "a";
                a.Tag = "A";
                w.Content = "w";
                w.Tag = "W";
                z.Content = "z";
                z.Tag = "Z";
                OemComma.Content = ",";
                OemComma.Tag = "OemComma";
                m.Content = "m";
                m.Tag = "M";
            }
        }

        
	}
}
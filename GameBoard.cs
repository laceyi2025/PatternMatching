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

namespace PatternMatching
{
   public partial class GameBoard : Form
   {
      // Constants
      const string CustomCOLORfilename = @"Configs\CustomColors.txt";
      const string DefaultCOLORfilename = @"Configs\DefaultColors.txt";

      // Global variables
      bool playerTurn = true; // Star wars = true : Star Trek = false
      string[,] theBoard = new string[8, 9];  // [rows, columns]
      int row = -1;
      int column = -1;

      DiagnosticWindow diagWindow;
      bool viewArrayContents = false;
      bool viewPatternBuild = false;

      public GameBoard()
      {
         InitializeComponent();
      }

      /// <summary>
      /// This method clears the Game Board Array in preparation for a new game
      /// </summary>
      private void ClearTheBoardArray()
      {
         for (int row = 0; row < theBoard.GetLength(0); row++)
         {
            for (int col = 0; col < theBoard.GetLength(1); col++)
            {
               theBoard[row, col] = "-";
            }
         }
      }

      /// <summary>
      /// This method resets the game board based on the type of reset specified.
      /// </summary>
      /// <param name="typeOfReset">"Full" means all properties will be set to default.
      ///                           "Disable" means disable the board at game win.</param>
      private void ResetBoard(string typeOfReset)
      {
         typeOfReset = typeOfReset.ToUpper();

         gbxGameBoard.Enabled = true;

         foreach (Control buttonControl in Controls["gbxGameBoard"].Controls)
         {
            if (buttonControl is Button)
            {
               if (typeOfReset == "DISABLE")
               {
                  buttonControl.Enabled = false;
               }
               else
               {
                  buttonControl.Enabled = true;
                  ((Button)buttonControl).FlatAppearance.BorderColor = Color.Black;
                  ((Button)buttonControl).Image = null;
               }
            }
         }
      }

      /// <summary>
      /// Resets the visual game board as well as the array representation of the board.
      /// </summary>
      private void ResetGameBoard()
      {
         ResetBoard("Full");
         ClearTheBoardArray();
      }
      private void mstrpitmExit_Click(object sender, EventArgs e)
      {
         this.Close();
      }

      /// <summary>
      /// Based on player turn determines which image to display
      /// </summary>
      /// <returns>Star Wars image or Star Trek image</returns>
      private Bitmap SetImage()
      {
         return (playerTurn) ? Properties.Resources.Star_Wars : Properties.Resources.Star_Trek;
      }

      /// <summary>
      /// Outlined the clicked button in red to indicate it is no longer available
      /// </summary>
      /// <param name="buttonToChange">The button control that was clicked</param>
      private void MarkButtonUsed(Button buttonToChange)
      {
         buttonToChange.FlatAppearance.BorderColor = Color.Red;
      }

      /// <summary>
      /// Based on player turn determines the value to be placed in the array 
      /// </summary>
      /// <returns>"W" for Stars Wars player, "T" for the Star Trek Player</returns>
      private string SetPlayerValue()
      {
         return (playerTurn) ? "W" : "T";
      }

      /// <summary>
      /// Updates the array representing the game board with appropriate value
      /// </summary>
      /// <param name="rowToUse">Row of the button clicked</param>
      /// <param name="columnToUse">Column of the button clicked</param>
      private void UpdateTheBoardArray(int rowToUse, int columnToUse)
      {
         theBoard[rowToUse, columnToUse] = SetPlayerValue();

         // Show contents if array diagnostics are enabled.
         if (viewArrayContents)
         {
            diagWindow.ClearDisplay();
            diagWindow.DisplayArray(theBoard);
         }
      }


      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private string GenerateRowPattern()
      {
         string pattern = "";

         for (int columnIndex = 0; columnIndex < theBoard.GetLength(1); columnIndex++)
         {
            pattern += theBoard[row, columnIndex];
         }

         //if the diag window is set to using view pattern, then display pattern
         if (viewPatternBuild)
         {

            diagWindow.DisplayLine("Row pattern: " + pattern);
         }
        
         return pattern;
      }

      private string GenerateColumnPattern()
      {
         return "";
      }

      private string GenerateDiagonal1Pattern()
      {

         return "";
      }

      private string GenerateDiagonal2Upper(int rowStart, int columnStart)
      {
         string pattern = "";

         rowStart++;
         columnStart++;

         while (rowStart < theBoard.GetLength(0) && columnStart < theBoard.GetLength(1))
         {
            pattern += theBoard[rowStart, columnStart];

            rowStart++;
            columnStart++;
         }
         return pattern;
      }

      private string GenerateDiagonal2Lower(int rowStart, int columnStart)
      {
         string pattern = "";

         rowStart--;
         columnStart--; 

         while (rowStart >= 0 && columnStart >= 0)
         {
            pattern = theBoard[rowStart, columnStart] + pattern;

            rowStart--;
            columnStart--;
         }
         return pattern;
      }

      private string GenerateDiagonal2Pattern()
      {
         string partUpper = GenerateDiagonal2Upper(row, column);
         string partCenter = theBoard[row, column];
         string partLower = GenerateDiagonal2Lower(row,column);

         string pattern = partLower + partCenter + partUpper;



         if (viewPatternBuild)
         {
            diagWindow.DisplayLine(Environment.NewLine);
            diagWindow.DisplayLine("Diagonal #2: ");
            diagWindow.DisplayLine("Upper : " + partUpper);
            diagWindow.DisplayLine("Center: " + partCenter);
            diagWindow.DisplayLine("Lower : " + partLower);
            diagWindow.DisplayLine("All   : " + pattern);
         }

         return "";
      }

      private bool CheckWinner()
      {
         string rowPattern = GenerateRowPattern();
         string columnPattern = GenerateColumnPattern();
         string diagonal1Pattern = GenerateDiagonal1Pattern();
         string diagonal2Pattern = GenerateDiagonal2Pattern();

         if (rowPattern.Contains("WWWW") || rowPattern.Contains("TTTT"))
         {
            return true;
         }
         else if (columnPattern.Contains("WWWW") || columnPattern.Contains("TTTT"))
         {
            return true;
         }
         else if (diagonal1Pattern.Contains("WWWW") || diagonal1Pattern.Contains("TTTT"))
         {
            return true;
         }
         else if (diagonal2Pattern.Contains("WWWW") || diagonal2Pattern.Contains("TTTT"))
         {
            return true;
         }
         else
         {
            return false;
         }

      }

      private void GameTurnClick(object sender, EventArgs e)
      {
         Button whichButtonClicked = (Button)sender;

         if (whichButtonClicked.Image == null)
         {
            row = int.Parse(whichButtonClicked.Name.Substring(3, 1));
            column = int.Parse(whichButtonClicked.Name.Substring(4, 1));

            /*
             * DONE - Set image
             * DONE - Disable button
             * DONE - Updated the Array
             * Check for Winner
             * if winner
             *   Announce
             *   Disable the board
             * else
             *   DONE - Change Turn
             */

            whichButtonClicked.Image = SetImage();

            MarkButtonUsed(whichButtonClicked);

            UpdateTheBoardArray(row, column);

            if (viewPatternBuild) diagWindow.ClearDisplay();

            if (CheckWinner())
            {
               // Announce Winner
               ResetBoard("Disable");
            }
            else
            {
               playerTurn = !playerTurn;
            }
         }
      }

      private void mstrpitmReset_Click(object sender, EventArgs e)
      {
         mstrpitmReset.Text = "Reset";

         ResetGameBoard();
      }

      private void GameBoard_Load(object sender, EventArgs e)
      {
         // Change the rest menu option to say "Start" for the initial game start.
         mstrpitmReset.Text = "Start";

         LoadColorFile();

         ResetGameBoard();
      }

      private void subdiagExamineArray_Click(object sender, EventArgs e)
      {
         viewPatternBuild = false;
         viewArrayContents = true;

         diagWindow.Text = "Examine Array Contents";
         //MessageBox.Show("Viewing array contents has been enabled.");
      }

      private void subdiagExaminePatternBuild_Click(object sender, EventArgs e)
      {
         viewPatternBuild = true;
         viewArrayContents = false;

         diagWindow.Text = "Examine Pattern Build";
         diagWindow.ClearDisplay();
         // MessageBox.Show("Viewing pattern build has been enabled.");
      }

      /// <summary>
      /// Enable diagnostics for the game board
      /// </summary>
      private void EnableDiagnositics()
      {
         // Turn Diagnostics on in the program.
         subdiagEnable.Text = "Disable";

         subdiagExamineArray.Enabled = true;
         subdiagExaminePatternBuild.Enabled = true;

         // Default to viewing the array
         viewArrayContents = true;
         // MessageBox.Show("Viewing array contents has been enabled.");

         // Instantiate the diagnostic window, set location and display
         diagWindow = new DiagnosticWindow();

         diagWindow.Text = "Examine Array Contents";

         diagWindow.StartPosition = FormStartPosition.Manual;
         diagWindow.Location = new Point(this.Location.X - 475, this.Location.Y);

         diagWindow.Show();

         // display the current contents of the array
         diagWindow.DisplayArray(theBoard);
      }

      /// <summary>
      /// Disable diagnostics for the game board.
      /// </summary>
      private void DisableDiagnositics()
      {
         // Close and dispose the open diagnostics window
         diagWindow.Close();
         diagWindow.Dispose();

         // Disable all diagnostic features
         subdiagEnable.Text = "Enable";

         subdiagExamineArray.Enabled = false;
         subdiagExaminePatternBuild.Enabled = false;

         viewArrayContents = false;
         viewPatternBuild = false;
      }

      private void subdiagEnable_Click(object sender, EventArgs e)
      {
         if (subdiagEnable.Text == "Enable")
         {
            EnableDiagnositics();
         }
         else
         {
            DisableDiagnositics();
         }
      }

      /// <summary>
      /// Load a color picker for the user to select a color
      /// </summary>
      /// <param name="currentColor">Current color in use</param>
      /// <returns>New color to use or the current color if canceled</returns>
      private Color GetColor(Color currentColor)
      {

         if (colorPicker.ShowDialog() == DialogResult.OK)
         {
            return colorPicker.Color;
         }
         else
         {
            return currentColor;
         }

      }

      private void subcolorMenuStrip_Click(object sender, EventArgs e)
      {
         mstrpMain.BackColor = GetColor(mstrpMain.BackColor);
      }

      private void subcolorFormBackground_Click(object sender, EventArgs e)
      {
         this.BackColor = GetColor(this.BackColor);
      }

      private void subcolorGroupBoxBackground_Click(object sender, EventArgs e)
      {
         gbxGameBoard.BackColor = GetColor(gbxGameBoard.BackColor);
      }

      /// <summary>
      /// Sets all buttons to a new back color
      /// </summary>
      /// <param name="newColor">Color to use</param>
      private void ChangeBackColorOfButtons(Color newColor)
      {
         foreach (Control buttonControl in Controls["gbxGameBoard"].Controls)
         {
            if (buttonControl is Button)
               buttonControl.BackColor = newColor;
         }
      }

      private void subcolorButtonBackground_Click(object sender, EventArgs e)
      {
         Color newButtonColor = GetColor(btn00.BackColor);

         ChangeBackColorOfButtons(newButtonColor);
      }

      private void subcolorSaveCustom_Click(object sender, EventArgs e)
      {
         StreamWriter customColorFile = File.CreateText(CustomCOLORfilename);

         customColorFile.WriteLine("Form: " + this.BackColor.ToString());
         customColorFile.WriteLine("Menu: " + mstrpMain.BackColor.ToString());
         customColorFile.WriteLine("Group: " + gbxGameBoard.BackColor.ToString());
         customColorFile.WriteLine("Button: " + btn00.BackColor.ToString());

         customColorFile.Close();
      }

      /// <summary>
      /// Determines which color file to open.  Default is used if a custom file is not found.
      /// </summary>
      /// <returns>Filename of color file to use</returns>
      private string DetermineColorFile()
      {
         return (File.Exists(CustomCOLORfilename)) ? CustomCOLORfilename : DefaultCOLORfilename;
      }

      /// <summary>
      /// Extract the Color to used from passed line from the color file
      /// </summary>
      /// <param name="colorLine">Line read from the color file</param>
      /// <returns>Color to be used on the control</returns>
      private Color ExtractColor(string colorLine)
      {
         string colorToUse = colorLine.Substring(colorLine.IndexOf('[') + 1).Trim(']');

         // Determine if the save file is use color name or the ARGB values.
         if (colorToUse.Contains("A="))
         {
            // Parse out the compoments
            string[] argbParts = colorToUse.Split(',');

            // Extract the data
            int alpha = int.Parse(argbParts[0].Trim(' ').Substring(2));
            int red = int.Parse(argbParts[1].Trim(' ').Substring(2));
            int green = int.Parse(argbParts[2].Trim(' ').Substring(2));
            int blue = int.Parse(argbParts[3].Trim(' ').Substring(2));

            // Return a color based on ARGB values
            return Color.FromArgb(alpha, red, green, blue);
         }
         else
         {
            // Return a color by name
            return Color.FromName(colorToUse);
         }
      }

      /// <summary>
      /// Load the color scheme for the program
      /// </summary>
      private void LoadColorFile()
      {
         StreamReader colorsToLoad = File.OpenText(DetermineColorFile());

         this.BackColor = ExtractColor(colorsToLoad.ReadLine());
         mstrpMain.BackColor = ExtractColor(colorsToLoad.ReadLine());
         gbxGameBoard.BackColor = ExtractColor(colorsToLoad.ReadLine());
         ChangeBackColorOfButtons(ExtractColor(colorsToLoad.ReadLine()));

         colorsToLoad.Close();
      }

      private void subcolorRestoreColor_Click(object sender, EventArgs e)
      {
         if (File.Exists(CustomCOLORfilename))
            File.Delete(CustomCOLORfilename);

         LoadColorFile();
      }
   }
}

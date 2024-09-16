using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;

using MySql.Data.MySqlClient;
using ZstdSharp.Unsafe;



namespace PredictValueIndexGame
{
    public partial class GameForm : Form
    {

        private List<int> numbers; 
        private Random random = new Random();
        private int selectedIndex;
        private int selectedNumber;
        private Dictionary<int, string> comboBoxItems = new Dictionary<int, string>();
        private Stopwatch userStopwatch; // Stopwatch to track user time
        public GameForm()
        {
            InitializeComponent();
            btnReset.Click += btnReset_Click;
            // Set the target number label's background color to transparent
            lblTargetNumber.BackColor = Color.Transparent;
            // Set the answer label's background color to transparent
            lblResult.BackColor = Color.Transparent;

        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {

            GenerateNumbers();
            SelectRandomNumber();
            PopulateChoices();
            PerformSearchAlgorithms();

            // Start the stopwatch to measure user's time
            userStopwatch = Stopwatch.StartNew();

        }

        private void GenerateNumbers()
        {
            numbers = new List<int>();
            for (int i = 0; i < 5000; i++)
            {
                numbers.Add(random.Next(1, 1000000));
            }
            numbers.Sort(); // Sorting for search algorithms
        }

        private void SelectRandomNumber()
        {
            selectedIndex = random.Next(numbers.Count);
            selectedNumber = numbers[selectedIndex];
            // Display the selected number in the label
            lblTargetNumber.Text = $"Select index for {selectedNumber}";
        }

        private void PopulateChoices()
        {
            if (cmbChoices == null)
            {
                MessageBox.Show("ComboBox not initialized.");
                return;
            }

            comboBoxItems.Clear();
            cmbChoices.Items.Clear();

            // Ensure selectedIndex and selectedNumber are valid
            if (numbers == null || numbers.Count == 0)
            {
                MessageBox.Show("Number list is empty.");
                return;
            }

            // Add the correct index
            comboBoxItems.Add(selectedIndex, selectedIndex.ToString());
            cmbChoices.Items.Add(selectedIndex.ToString());

            var incorrectIndices = new HashSet<int>();

            while (incorrectIndices.Count < 3)
            {
                int incorrectIndex;
                do
                {
                    incorrectIndex = random.Next(numbers.Count);
                } while (incorrectIndices.Contains(incorrectIndex) || incorrectIndex == selectedIndex);

                incorrectIndices.Add(incorrectIndex);
            }

            foreach (var index in incorrectIndices)
            {
                comboBoxItems.Add(index, index.ToString());
                cmbChoices.Items.Add(index.ToString());
            }

            // Shuffle the choices
            var items = cmbChoices.Items.Cast<string>().OrderBy(x => random.Next()).ToList();
            cmbChoices.Items.Clear();
            foreach (var item in items)
            {
                cmbChoices.Items.Add(item);
            }

            cmbChoices.SelectedIndex = -1; // Clear selection
        }




        private void PerformSearchAlgorithms()
        {
            var stopwatch = new Stopwatch();

            // Measure time for Binary Search
            stopwatch.Start();
            var binarySearchResult = SearchAlgorithms.BinarySearch(numbers, selectedNumber);
            stopwatch.Stop();
            long binarySearchTimeMicros = stopwatch.ElapsedTicks / (Stopwatch.Frequency / 1000000); // Convert to microseconds

            // Measure time for Jump Search
            stopwatch.Restart();
            var jumpSearchResult = SearchAlgorithms.JumpSearch(numbers, selectedNumber);
            stopwatch.Stop();
            long jumpSearchTimeMicros = stopwatch.ElapsedTicks / (Stopwatch.Frequency / 1000000); // Convert to microseconds

            // Measure time for Exponential Search
            stopwatch.Restart();
            var exponentialSearchResult = SearchAlgorithms.ExponentialSearch(numbers, selectedNumber);
            stopwatch.Stop();
            long exponentialSearchTimeMicros = stopwatch.ElapsedTicks / (Stopwatch.Frequency / 1000000); // Convert to microseconds

            // Measure time for Fibonacci Search
            stopwatch.Restart();
            var fibonacciSearchResult = SearchAlgorithms.FibonacciSearch(numbers, selectedNumber);
            stopwatch.Stop();
            long fibonacciSearchTimeMicros = stopwatch.ElapsedTicks / (Stopwatch.Frequency / 1000000); // Convert to microseconds

            // Measure time for Interpolation Search
            stopwatch.Restart();
            var interpolationSearchResult = SearchAlgorithms.InterpolationSearch(numbers, selectedNumber);
            stopwatch.Stop();
            long interpolationSearchTimeMicros = stopwatch.ElapsedTicks / (Stopwatch.Frequency / 1000000); // Convert to microseconds

            // Save results to the database
            //SaveResult(txtUserName.Text, "Binary Search", selectedNumber, binarySearchResult.index, binarySearchTimeMicros, binarySearchResult.index == selectedIndex);
            // SaveResult(txtUserName.Text, "Jump Search", selectedNumber, jumpSearchResult.index, jumpSearchTimeMicros, jumpSearchResult.index == selectedIndex);
            ////SaveResult(txtUserName.Text, "Exponential Search", selectedNumber, exponentialSearchResult.index, exponentialSearchTimeMicros, exponentialSearchResult.index == selectedIndex);
            //SaveResult(txtUserName.Text, "Fibonacci Search", selectedNumber, fibonacciSearchResult.index, fibonacciSearchTimeMicros, fibonacciSearchResult.index == selectedIndex);
            // SaveResult(txtUserName.Text, "Interpolation Search", selectedNumber, interpolationSearchResult.index, interpolationSearchTimeMicros, interpolationSearchResult.index == selectedIndex);
            SaveResult("Binary Search", selectedNumber, binarySearchResult.index, binarySearchTimeMicros);
            SaveResult("Jump Search", selectedNumber, jumpSearchResult.index, jumpSearchTimeMicros);
            SaveResult("Exponential Search", selectedNumber, exponentialSearchResult.index, exponentialSearchTimeMicros);
            SaveResult("Fibonacci Search", selectedNumber, fibonacciSearchResult.index, fibonacciSearchTimeMicros);
            SaveResult("Interpolation Search", selectedNumber, interpolationSearchResult.index, interpolationSearchTimeMicros);

            //Optionally, display algorithem search results in the label
            // lblResult.Text = $"Binary Search - Index: {binarySearchResult.index}, Time: {binarySearchTimeMicros} µs\n" +
            //  $"Jump Search - Index: {jumpSearchResult.index}, Time: {jumpSearchTimeMicros} µs\n" +
            //  $"Exponential Search - Index: {exponentialSearchResult.index}, Time: {exponentialSearchTimeMicros} µs\n" +
            // $"Fibonacci Search - Index: {fibonacciSearchResult.index}, Time: {fibonacciSearchTimeMicros} µs\n" +
            // $"Interpolation Search - Index: {interpolationSearchResult.index}, Time: {interpolationSearchTimeMicros} µs";


            //showing algorithem and the time
            lbSearchResult.Text = $"Binary Search -  Time: {binarySearchTimeMicros} µs\n" +
                            $"Jump Search - Time: {jumpSearchTimeMicros} µs\n" +
                            $"Exponential Search -  Time: {exponentialSearchTimeMicros} µs\n" +
                            $"Fibonacci Search -  Time: {fibonacciSearchTimeMicros} µs\n" +
                            $"Interpolation Search - Time: {interpolationSearchTimeMicros} µs";
        }







        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (cmbChoices.SelectedItem == null)
            {
                MessageBox.Show("Please select an index.");
                return;
            }

            string selectedText = cmbChoices.SelectedItem.ToString();
            int userChoiceIndex = comboBoxItems.FirstOrDefault(x => x.Value == selectedText).Key;

            // Stop the stopwatch and capture the time taken by the user
            long timeTakenMicros = userStopwatch != null ? userStopwatch.ElapsedTicks / (Stopwatch.Frequency / 1000000) : 0;
            userStopwatch?.Stop();

            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("Please enter your name.");
                return;
            }

            bool isCorrect = userChoiceIndex == selectedIndex;

            lblResult.Text = isCorrect
                ? $"Spot on! You got it right! The index is {selectedIndex}."
                : $"Not quite right, give it another shot!";

            // Save the search results regardless of correctness  //this is not
            //SaveSearchResult(txtUserName.Text, "User's Choice", selectedNumber, userChoiceIndex, timeTakenMicros, isCorrect);

            // Save the user's prediction only if it's correct //this is working 
            if (isCorrect)
            {
                SaveUserPrediction(txtUserName.Text, isCorrect);
            }
        }



        private void SaveSearchResult(string userName, string searchMethod, int targetValue, int resultIndex, long timeTaken, bool isCorrect)
        {
            string connectionString = "Server=localhost;Database=search_game_db;User ID=root;Password=pavani;Connection Timeout=30;";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    var command = new MySqlCommand(
                        "INSERT INTO search_results (user_name, search_method, target_value, result_index, time_taken, is_correct, search_date) VALUES (@UserName, @SearchMethod, @TargetValue, @ResultIndex, @TimeTaken, @IsCorrect, @SearchDate)",
                        connection
                    );

                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@SearchMethod", searchMethod);
                    command.Parameters.AddWithValue("@TargetValue", targetValue);
                    command.Parameters.AddWithValue("@ResultIndex", resultIndex);
                    command.Parameters.AddWithValue("@TimeTaken", timeTaken);
                    command.Parameters.AddWithValue("@IsCorrect", isCorrect);
                    command.Parameters.AddWithValue("@SearchDate", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving results: " + ex.Message);
            }
        }
        

        /* private void SaveResult(string userName, string searchMethod, int targetValue, int resultIndex, long timeTaken, bool isCorrect)
         {
             string connectionString = "Server=localhost;Database=search_game_db;User ID=root;Password=pavani;Connection Timeout=30;";

             try
             {
                 using (var connection = new MySqlConnection(connectionString))
                 {
                     connection.Open();

                     var command = new MySqlCommand("INSERT INTO search_results (user_name, search_method, target_value, result_index, time_taken, is_correct, search_date) VALUES (@UserName, @SearchMethod, @TargetValue, @ResultIndex, @TimeTaken, @IsCorrect, @SearchDate)", connection);

                     command.Parameters.AddWithValue("@UserName", userName);
                     command.Parameters.AddWithValue("@SearchMethod", searchMethod);
                     command.Parameters.AddWithValue("@TargetValue", targetValue);
                     command.Parameters.AddWithValue("@ResultIndex", resultIndex);
                     command.Parameters.AddWithValue("@TimeTaken", timeTaken);
                     command.Parameters.AddWithValue("@IsCorrect", isCorrect);
                     command.Parameters.AddWithValue("@SearchDate", DateTime.Now);

                     command.ExecuteNonQuery();
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show("An error occurred while saving results: " + ex.Message);
             }
         }*/

        private void SaveResult(string searchMethod, int targetValue, int resultIndex, long timeTaken)
        {
            string connectionString = "Server=localhost;Database=search_game_db;User ID=root;Password=pavani;Connection Timeout=30;";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Adjusted SQL command to exclude user_name and is_correct
                    var command = new MySqlCommand(
                        "INSERT INTO search_results (search_method, target_value, result_index, time_taken, search_date) VALUES (@SearchMethod, @TargetValue, @ResultIndex, @TimeTaken, @SearchDate)",
                        connection
                    );

                    // Adding parameters
                    command.Parameters.AddWithValue("@SearchMethod", searchMethod);
                    command.Parameters.AddWithValue("@TargetValue", targetValue);
                    command.Parameters.AddWithValue("@ResultIndex", resultIndex);
                    command.Parameters.AddWithValue("@TimeTaken", timeTaken);
                    command.Parameters.AddWithValue("@SearchDate", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving results: " + ex.Message);
            }
        }


        private void SaveUserPrediction(string userName, bool isCorrect)
        {
            string connectionString = "Server=localhost;Database=search_game_db;User ID=root;Password=pavani;Connection Timeout=30;";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    var command = new MySqlCommand(
                        "INSERT INTO UserPredictions (user_name, correct_answer, Timestamp, prediction_date) VALUES (@UserName, @CorrectAnswer, @Timestamp, @PredictionDate)",
                        connection
                    );

                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@CorrectAnswer", isCorrect ? "True" : "False");
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                    command.Parameters.AddWithValue("@PredictionDate", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving user prediction: " + ex.Message);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Clear the ComboBox selection
            cmbChoices.SelectedIndex = -1;

            // Clear the result label
            lblResult.Text = "";

            //clear the searchAlgorithem results label
            lbSearchResult.Text =  "";

            // Clear the result label
            lblTargetNumber.Text = "";

            // Clear the TextBox for the user's name
            txtUserName.Text = "";

            // Clear the ComboBox items
            cmbChoices.Items.Clear();

            // Stop the user stopwatch if it's running
            if (userStopwatch != null && userStopwatch.IsRunning)
            {
                userStopwatch.Stop();
            }

            // Reset game variables
            numbers = null;
            selectedIndex = -1;
            selectedNumber = -1;

            // Optionally, re-enable the Start button if needed
            btnStartGame.Enabled = true;

            // Clear the Dictionary storing ComboBox items
            comboBoxItems.Clear();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}

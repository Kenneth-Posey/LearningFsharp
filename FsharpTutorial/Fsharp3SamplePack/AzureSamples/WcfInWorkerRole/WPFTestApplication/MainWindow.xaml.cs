using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LoanCalculatorContracts;
using System.ServiceModel;

namespace WPFTestApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //The serviceUrl depends on the variable "endpoint" in workrole.fs file, you need to adjust the url if you run into a error.
        private string serviceUrl = "net.tcp://127.0.0.1:11000/LoanCalculator";
        private string DefaultResultText;

        // This is how we get a proxy to access the WCF service
        private ILoanCadulator GetAProxy()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            EndpointAddress endpointAddress
                = new EndpointAddress(serviceUrl);

            return new ChannelFactory<ILoanCadulator>
                (binding, endpointAddress).CreateChannel();
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs args)
        {
            cmbTotalLoan.Items.Clear();
            cmbAnualInterest.Items.Clear();
            cmbTermInMonth.Items.Clear();

            cmbTotalLoan.Items.Add("** Please select **");
            for (int i = 1; i <= 5; i++)
            {
                cmbTotalLoan.Items.Add(100000 * i);
            }
            cmbTotalLoan.SelectedIndex = 0;

            cmbAnualInterest.Items.Add("** Please select **");
            for (int i = 1; i <= 5; i++)
            {
                cmbAnualInterest.Items.Add(3 + i);
            }
            cmbAnualInterest.SelectedIndex = 0;

            cmbTermInMonth.Items.Add("** Please select **");
            for (int i = 1; i <= 5; i++)
            {
                cmbTermInMonth.Items.Add(12 * 10 * i);
            }
            cmbTermInMonth.SelectedIndex = 0;

            DefaultResultText = "Please select the amount, the interest rate,"
                + " the term and click the \"Calculate\" button";
            txtCalculationResult.Text = DefaultResultText;
        }

        private void cmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtCalculationResult.Text = DefaultResultText;
            txtCalculationResult.Foreground = Brushes.Green;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            string amount = cmbTotalLoan.SelectedItem.ToString();
            string interestRateInPercent = cmbAnualInterest.SelectedItem.ToString();
            string termInMonth = cmbTermInMonth.SelectedItem.ToString();

            if (amount == "** Please select **")
            {
                MessageBox.Show("Please select the total loan.");
                return;
            }

            if (interestRateInPercent == "** Please select **")
            {
                MessageBox.Show("Please select the anual interest rate");
                return;
            }

            if (termInMonth == "** Please select **")
            {
                MessageBox.Show("Please select the term");
                return;
            }

            LoanInformation loan = new LoanInformation(Convert.ToDouble(amount),Convert.ToDouble(interestRateInPercent),Convert.ToInt32(termInMonth));

            string resultText = null;

            try
            {
                PaymentInformation payment = GetAProxy().Calculate(loan);
                resultText = "Monthly payment: $"
                    + payment.MonthlyPayment.ToString()
                    + ", Total payment: $" + payment.TotalPayment.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when calculating the payments - " + ex.Message);
                return;
            }


            txtCalculationResult.Text = resultText;
            txtCalculationResult.Foreground = Brushes.Brown;
        }

        private void Calculate2_Click(object sender, RoutedEventArgs e)
        {
            string resultText;
            System.Tuple<int,string> tp = new System.Tuple<int, string>(3,"3");
            var result = GetAProxy().Calculate2(tp);
            resultText = result.Item2 + "is" + result.Item1.ToString();
            txtCalculationResult.Text = resultText;
            txtCalculationResult.Foreground = Brushes.Brown;
        }

        private void Calculate3_Click(object sender, RoutedEventArgs e)
        {
            string resultText = "";

            Suit result = GetAProxy().Calculate3(Suit.Club);
            if (result.IsDiamond)
                resultText = "result is Diamond";
            txtCalculationResult.Text = resultText;
            txtCalculationResult.Foreground = Brushes.Brown;
        }
    }
}

using System.Data;
using System.Windows;
using Diplom.TeacherFolder.Report;
using Microsoft.Reporting.WinForms;

namespace Diplom.TeacherFolder
{
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();

            PupilsVariantsDataSet dataSet = new PupilsVariantsDataSet();
            Report.PupilsVariantsDataSetTableAdapters.PupilsVariantsDataTableTableAdapter adapter = new Report.PupilsVariantsDataSetTableAdapters.PupilsVariantsDataTableTableAdapter();
            adapter.Fill(dataSet.PupilsVariantsDataTable);

            ReportDataSource dataSource = new ReportDataSource("PupilsVariantsDataSet", dataSet.PupilsVariantsDataTable as DataTable);
            PupilsReportViewer.LocalReport.DataSources.Add(dataSource);
            PupilsReportViewer.LocalReport.ReportEmbeddedResource = "Diplom.TeacherFolder.Report.PupilsVariantsReport.rdlc";
            PupilsReportViewer.RefreshReport();
        }
    }
}

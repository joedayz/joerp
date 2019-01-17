using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JOERP.Business.Logic;
using Microsoft.Reporting.WinForms;

namespace JOERP.Console
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var listado = ReporteBL.Instancia.ListarInventarioFisico(3, 0, 0, 0, "", 0, 0, "", 0);

            var reportDataSource = new ReportDataSource("DataSet1", listado);

            reportViewer1.LocalReport.DataSources.Add(reportDataSource);

            reportViewer1.RefreshReport();
        }
    }
}

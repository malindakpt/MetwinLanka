using BusinessLogic;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class StatisticsController : Controller
    {
        ReportManager rm = new ReportManager();
        // GET: /Statistics/
        public ActionResult Index()
        {
            return View(); 
        }

        public ActionResult IncomeSummary(DateTime? fromdate, DateTime? todate)
        {
            DateTime fdate = System.DateTime.Now.AddMonths(-12);
            DateTime tdate = System.DateTime.Now;

            if (todate.HasValue) tdate = todate.Value;
            if (fromdate.HasValue) fdate = fromdate.Value;

            var ms = rm.getSummary(fdate, tdate);
            var xData = ms.Select(i => i.month).ToArray();
            var yData = ms.Select(i => new object[]{i.income}).ToArray();
            var zData = ms.Select(i => new object[] { i.expences }).ToArray();

            var chart = new Highcharts("chart")
                .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
                .SetTitle(new Title { Text = "Monthly income" })
                .SetSubtitle(new Subtitle { Text = "income - expences" })
                .SetXAxis(new XAxis { Categories = xData })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "RS" } })
                .SetTooltip(new Tooltip
                {
                    Enabled = true,
                    Formatter = @"function() {return '<b>'+this.series.name+'</b><br/>'+this.x+':'+this.y;}"
                })
                .SetPlotOptions(new PlotOptions
                {
                    Line = new PlotOptionsLine
                    {
                        DataLabels = new PlotOptionsLineDataLabels
                        {
                            Enabled = true
                        },
                        EnableMouseTracking = false
                    }
                })
                .SetSeries(new[]{
                    new Series{Name = "Income",Data = new Data(yData)},
                    new Series{Name = "Expences",Data = new Data(zData)},
                });

                

            return View(chart);
        }

        public ActionResult SalesAreaSummary(DateTime? fromdate, DateTime? todate)
        {
            DateTime fdate = System.DateTime.Now.AddMonths(-12);
            DateTime tdate = System.DateTime.Now;

            if (todate.HasValue) tdate = todate.Value;
            if (fromdate.HasValue) fdate = fromdate.Value;

            var ms = rm.getSaleSummary(fdate, tdate);

            if (ms.ToArray().Length == 0)
            {
                ViewBag.status = "OK";
            }
            else
            {
                var xData = ms.First().monthSummary.Select(m => m.month).ToArray();
                var yData = ms.Select(i => i.area).Distinct().ToArray();
                //var zData = ms.Select(i => new object[] { i.areaSummary.Select(k =>k.sales) } ).ToArray();

                var chart = new Highcharts("chart")
                    .InitChart(new Chart { DefaultSeriesType = ChartTypes.Line })
                    .SetTitle(new Title { Text = "Monthly Sales by Areas" })
                    .SetSubtitle(new Subtitle { Text = fdate.Year.ToString() + " " + fdate.ToString("MMM") + " to " + fdate.Year.ToString() + " " + tdate.ToString("MMM") })
                    .SetXAxis(new XAxis { Categories = xData })
                    .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "RS" } })
                    .SetTooltip(new Tooltip
                    {
                        Enabled = true,
                        Formatter = @"function() {return '<b>'+this.series.name+'</b><br/>'+this.x+':'+this.y;}"
                    })
                    .SetPlotOptions(new PlotOptions
                    {
                        Line = new PlotOptionsLine
                        {
                            DataLabels = new PlotOptionsLineDataLabels
                            {
                                Enabled = true
                            },
                            EnableMouseTracking = false
                        }
                    })
                    .SetSeries(ms.Select(
                        z => new Series
                        {
                            Name = z.area,
                            Data = new Data(z.monthSummary.Select(s => (object)s.value).ToArray())
                        }).ToArray());
                //.SetSeries(new[]
                //{
                //    new Series{Name = "Sales",Data = new Data(zData)},
                //});
                if (!fromdate.HasValue && !todate.HasValue)
                {
                    ViewBag.status = "OK";
                }
                else
                {
                    ViewBag.status = "OOK";
                }
                return View(chart);
            }

            return View();

            
        }

	}
}
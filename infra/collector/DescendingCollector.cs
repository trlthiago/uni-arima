
using System.Collections.Generic;
using System.Linq;
using uni_elastic_manager.Model;

namespace uni_elastic_manager.infra
{
    public class DescendingCollector : IMetricCollector
    {
        public List<CpuMetric> Collect()
        {
            var metrics = new double[] { 600, 595, 590, 585, 580, 575, 570, 565, 560, 555, 550, 545, 540, 535, 530, 525, 520, 515, 510, 505, 500, 495, 490, 485, 480, 475, 470, 465, 460, 455, 450, 445, 440, 435, 430, 425, 420, 415, 410, 405, 400, 395, 390, 385, 380, 375, 370, 365, 360, 355, 350, 345, 340, 335, 330, 325, 320, 315, 310, 305, 300, 295, 290, 285, 280, 275, 270, 265, 260, 255, 250, 245, 240, 235, 230, 225, 220, 215, 210, 205, 200, 195, 190, 185, 180, 175, 170, 165, 160, 155, 150, 145, 140, 135, 130, 125, 120, 115, 110, 105, 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5 };
            return metrics.Select(x => new CpuMetric { Value = x.ToString() }).ToList();
        }
    }
}
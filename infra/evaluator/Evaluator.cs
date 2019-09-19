using System;
using uni_elastic_manager;

namespace uni_arima.infra.evaluator
{
    public enum EvaluatorAction
    {
        Nothing = 1,
        AddResource = 2,
        RemoveResource = 3
    }

    public class Evaluator
    {
        protected readonly double _CPUThresholdUpper;
        protected readonly double _CPUThresholdLower;
        public Evaluator(Settings settings)
        {
            _CPUThresholdUpper = Convert.ToDouble(settings.CPUThresholdUpper);
            _CPUThresholdLower = Convert.ToDouble(settings.CPUThresholdLower);
        }

        public EvaluatorAction Evaluate(double value)
        {
            if (value > _CPUThresholdUpper)
            {

                return EvaluatorAction.AddResource;
            }
            else if (value < _CPUThresholdLower)
            {
                return EvaluatorAction.RemoveResource;
            }
            return EvaluatorAction.Nothing;
        }

    }
}
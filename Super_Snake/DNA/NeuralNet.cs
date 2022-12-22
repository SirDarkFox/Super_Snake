using Newtonsoft.Json;
using Super_Snake.Metrics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Snake.DNA
{
    public class NeuralNet
    {
        [JsonProperty]
        Random _random;
        [JsonProperty]
        Matrix[] _headenLayers;

        [JsonProperty]
        int _iNodes { get; set; }
        [JsonProperty]
        int _hNodes { get; set; }
        [JsonProperty]
        int _oNodes { get; set; }
        [JsonProperty]
        int _hLayers { get; set; }
        [JsonProperty]
        bool _bias { get; set; }

        [JsonProperty]
        BaseActivation _activation { get; set; }
        [JsonProperty]
        BaseCrossover _crossover { get; set; }
        [JsonProperty]
        BaseMutation _mutation { get; set; }

        public NeuralNet(int iNodes, int hNodes, int oNodes, int hLayers, bool bias,
             BaseActivation activation, BaseCrossover crossover, BaseMutation mutation, Random random)
        {
            _random = random;

            _iNodes = iNodes;
            _hNodes = hNodes;
            _oNodes = oNodes;
            _hLayers = hLayers;

            _activation = activation;
            _crossover = crossover;
            _mutation = mutation;

            _bias = bias;

            int index = (_bias) ? 1 : 0;
            _headenLayers = new Matrix[_hLayers + 1];
            _headenLayers[0] = new Matrix(_hNodes, _iNodes + index, _random);
            for (int i = 1; i < _hLayers; i++)
            {
                _headenLayers[i] = new Matrix(_hNodes, _hNodes + index, _random);
            }
            _headenLayers[_headenLayers.Length - 1] = new Matrix(_oNodes, _hNodes + index, _random);

            foreach (Matrix layer in _headenLayers)
            {
                layer.Randomize();
            }
        }

        public void Update(BaseActivation baseActivation, BaseCrossover baseCrossover, BaseMutation baseMutation)
        {
            _activation = baseActivation;
            _crossover = baseCrossover;
            _mutation = baseMutation;
        }

        public double[] Output(double[] inputsArr)
        {
            Matrix inputs = new Matrix(inputsArr);

            for (int i = 0; i < _hLayers; i++)
            {
                if (_bias)
                {
                    inputs = inputs.AddBias();
                }

                Matrix hiddenInput = _headenLayers[i].Dot(inputs);
                Matrix hiddenOutput = _activation.Activate(hiddenInput, _random);
                inputs = hiddenOutput;
            }

            if (_bias)
            {
                inputs = inputs.AddBias();
            }

            Matrix lastInput = _headenLayers[_headenLayers.Length - 1].Dot(inputs);
            Matrix output = _activation.Activate(lastInput, _random);

            return output.ToArray();
        }

        public void Mutate(double mutationRate)
        {
            foreach (Matrix hLayer in _headenLayers)
            {
                _mutation.Mutate(hLayer, mutationRate, _random);
            }
        }

        public NeuralNet DoCrossover(NeuralNet partner)
        {
            NeuralNet child = new NeuralNet(_iNodes, _hNodes, _oNodes, _hLayers, _bias, 
                _activation, _crossover, _mutation, _random);

            for (int i = 0; i < _headenLayers.Length; i++)
            {
                child._headenLayers[i] = _crossover.DoCrossover(_headenLayers[i], partner._headenLayers[i], _random);
            }
            return child;
        }

        public void Load(Matrix[] weights)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = weights[i];
            }
        }

        public Matrix[] Pull()
        {
            Matrix[] model = new Matrix[_headenLayers.Length];
            for (int i = 0; i < _headenLayers.Length; i++)
            {
                model[i] = _headenLayers[i];
            }
            return model;
        }

        public NeuralNet Clone()
        {
            NeuralNet clone = new NeuralNet(_iNodes, _hNodes, _oNodes, _hLayers, _bias, 
                _activation, _crossover, _mutation, _random);

            for (int i = 0; i < _headenLayers.Length; i++)
            {
                clone._headenLayers[i] = _headenLayers[i].Clone();
            }

            return clone;
        }
    }
}

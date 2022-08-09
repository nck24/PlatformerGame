using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameTest.AI_code
{
    // A node class which all the other node classes will inherit from
    public abstract class Node
    {
        protected float NodeValue;
        protected float bias;
        protected Func<float, float> EndFunction;

        /// <summary>
        /// Get a float that represents the end node value
        /// </summary>
        /// <returns></returns>
        public abstract float OutputNodeValue();

        /// <summary>
        /// Manualy set the node value
        /// </summary>
        /// <param name="newNodeValue"></param>
        public abstract void SetNodeValue(float newNodeValue);

        /// <summary>
        /// Calculates the node value and updates it
        /// </summary>
        public abstract void UpdateNodeValue();

        /// <summary>
        /// Sums values from previous nodes and weights
        /// </summary>
        /// <param name="values"></param>
        public abstract void SetSumVal(float[] values);
    }

    public class InputNode : Node
    {
        public InputNode(){
            this.EndFunction = value => value;
            this.NodeValue = float.NaN;
            this.bias = 0f;
        }

        public override void SetNodeValue(float newNodeValue){
            this.NodeValue = newNodeValue;
        }

        public override float OutputNodeValue(){
            return EndFunction.Invoke(NodeValue);
        }

        public override void UpdateNodeValue()
        {
            // do nothing
        }

        public override void SetSumVal(float[] values)
        {
            // do nothing
        }
    }

    public class HiddenNode : Node
    {
        float sumValue;

        public HiddenNode(Func<float, float> function){
            this.bias = float.NaN;
            this.sumValue = float.NaN;
            this.EndFunction = function;
        }

        public HiddenNode(Func<float, float> function, float bias){
            this.bias = bias;
            this.sumValue = float.NaN;
            this.EndFunction = function;
        }

        public void SetEndFunction(Func<float, float> function){
            this.EndFunction = function;
        }

        public void SetBias(float newBias){
            this.bias = newBias;
        }

        public override void SetSumVal(float[] values){
            sumValue = values.Sum(input => input);
        }

        public override float OutputNodeValue(){
            this.UpdateNodeValue();
            return this.EndFunction.Invoke(NodeValue);
        }

        public override void SetNodeValue(float newNodeValue)
        {
            this.NodeValue = newNodeValue;
        }

        public override void UpdateNodeValue()
        {
            SetNodeValue(this.sumValue - this.bias);
        }

    }

    public class OutputNode : Node
    {
        float sumValue;
        Func<float, bool> resultFunction;

        public OutputNode(Func<float, float> function, Func<float, bool> resultFunction){
            this.bias = float.NaN;
            this.sumValue = float.NaN;
            this.EndFunction = function;
            this.resultFunction = resultFunction;
        }

        public OutputNode(Func<float, float> function, Func<float, bool> resultFunction, float bias){
            this.bias = bias;
            this.sumValue = float.NaN;
            this.EndFunction = function;
            this.resultFunction = resultFunction;
        }

        public void SetEndFunction(Func<float, float> function){
            this.EndFunction = function;
        }

        public void SetResultFunction(Func<float, bool> function){
            this.resultFunction = function;
        }

        public void SetBias(float newBias){
            this.bias = newBias;
        }

        /// <summary>
        /// If returns true action is executed else not
        /// </summary>
        /// <returns></returns>
        public bool GetResult(){
            return resultFunction.Invoke(this.OutputNodeValue());
        }

        public override void SetSumVal(float[] values){
            sumValue = values.Sum(input => input);
        }

        public override float OutputNodeValue(){
            this.UpdateNodeValue();
            return this.EndFunction.Invoke(NodeValue);
        }

        public override void SetNodeValue(float newNodeValue){
            this.NodeValue = newNodeValue;
        }

        public override void UpdateNodeValue()
        {
            SetNodeValue(this.sumValue - this.bias);
        }
    }

    public class Weight
    {
        float multiplier;

        public Weight(float multiplayer){
            this.multiplier = multiplayer;
        }

        public void ChangeMultiplayer(float newMultiplayer){
            this.multiplier = newMultiplayer;
        }

        public float MultiplayNode(Node node){
            return node.OutputNodeValue() * multiplier;
        }
    }

    public class Sinapse
    {
        Node masterNode;
        Node[] previousNodes;
        Weight[] weightsToMasterNode;

        public Sinapse(Node masterNode, Node[] previousNodes, Weight[]weights){
            this.masterNode = masterNode;
            this.previousNodes = previousNodes;
            this.weightsToMasterNode = weights;
        }

        public float[] GetValuesForSum(){
            float[] values = new float[previousNodes.Length];
            for (int i = 0; i < values.Length; i++){
                values[i] = weightsToMasterNode[i].MultiplayNode(previousNodes[i]);
            }
            return values;
        }

        public void SumMasterNodeValue(){
            masterNode.SetSumVal( this.GetValuesForSum() );
            masterNode.UpdateNodeValue();
        }
    }

    public class NeuralNetwork
    {
        InputNode[] inputs;
        HiddenNode[,] hiddenLayers;
        OutputNode[] outputs;
        List<Sinapse[]> sinapses;
        Func<float, float> EndFunction;
        Func<float, float> EndOutputFunction;
        Func<float, bool> ActionFunction;

        public NeuralNetwork(int inputCount, int depth, int hiddenLayerSize, int outputCount){
            Random rand = new Random();
            this.sinapses = new List<Sinapse[]>();
            this.inputs = new InputNode[inputCount];
            for (int i = 0; i < inputCount; i++){
                this.inputs[i] = new InputNode();
            }

            this.hiddenLayers = new HiddenNode[depth, hiddenLayerSize];
            for (int i = 0; i < depth; i++){
                for (int j = 0; j < hiddenLayerSize; j++){
                    this.hiddenLayers[i, j] = new HiddenNode(EndFunction);
                }
            }

            this.outputs = new OutputNode[outputCount];
            for (int i = 0; i < outputCount; i++){
                this.outputs[i] = new OutputNode(EndFunction, ActionFunction);
            }

            for (int i = 0; i < depth; i++){
                for (int j = 0; j < hiddenLayerSize; j++){
                    this.hiddenLayers[i, j].SetBias(rand.Next(-20, 20) * rand.NextSingle());
                }
            }

            for (int i = 0; i < outputCount; i++){
                this.outputs[i].SetBias(rand.Next(-20, 20) * rand.NextSingle());
            }

            Sinapse[] tempSinapseArr = new Sinapse[hiddenLayerSize];
            Weight[] tempWeightsArr = new Weight[hiddenLayerSize];
            for (int i = 0; i < hiddenLayerSize; i++){
                for (int j = 0; j < hiddenLayerSize; j++){
                    tempWeightsArr[j] = new Weight( rand.Next(-10, 10) * rand.NextSingle() );
                }
                tempSinapseArr[i] = new Sinapse(this.hiddenLayers[0, i], this.inputs, tempWeightsArr);
                tempWeightsArr = new Weight[hiddenLayerSize];
            }
            this.sinapses.Add(tempSinapseArr);
            tempSinapseArr = new Sinapse[hiddenLayerSize];

            for (int i = 1; i < depth; i++){
                for (int k = 0; k < hiddenLayerSize; k++){
                    for (int j = 0; j < hiddenLayerSize; j++){
                        tempWeightsArr[j] = new Weight( rand.Next(-10, 10) * rand.NextSingle() );
                    }
                    Node[] tempPreviousNodes = new Node[hiddenLayerSize];
                    for (int j = 0; j < hiddenLayerSize; j++){
                        tempPreviousNodes[j] = this.hiddenLayers[i - 1, j];
                    }
                    tempSinapseArr[k] = new Sinapse(this.hiddenLayers[i, k], tempPreviousNodes, tempWeightsArr);
                }
                this.sinapses.Add(tempSinapseArr);
                tempSinapseArr = new Sinapse[hiddenLayerSize];
                tempWeightsArr = new Weight[hiddenLayerSize];
            }
            
            tempSinapseArr = new Sinapse[outputs.Length];
            for (int i = 0; i < outputs.Length; i++){
                tempWeightsArr = new Weight[hiddenLayerSize];
                for (int j = 0; j < hiddenLayerSize; j++){
                    tempWeightsArr[j] = new Weight( rand.Next(-10, 10) * rand.NextSingle() );
                }
                Node[] tempPreviousNodes = new Node[hiddenLayerSize];
                for (int j = 0; j < hiddenLayerSize; j++){
                    tempPreviousNodes[j] = this.hiddenLayers[depth - 1, j];
                }

                tempSinapseArr[i] = new Sinapse(outputs[i], tempPreviousNodes, tempWeightsArr);
            }
            this.sinapses.Add(tempSinapseArr);
        }
    
        public void SetEndFuncton(Func<float, float> func){
            this.EndFunction = func;
            foreach (HiddenNode hn in hiddenLayers){
                hn.SetEndFunction(this.EndFunction);
            }
        }

        public void SetEndOutputFuncton(Func<float, float> func){
            this.EndOutputFunction = func;
            foreach (OutputNode on in outputs){
                on.SetEndFunction(this.EndOutputFunction);
            }
        }

        public void SetActionFunction(Func<float, bool> func){
            this.ActionFunction = func;
            foreach (OutputNode on in outputs){
                on.SetResultFunction(this.ActionFunction);
            }
        }

        public void CalcOutputs(){
            for (int i = 0; i < sinapses.Count; i++){
                for (int j = 0; j < sinapses[i].Length; j++){
                    sinapses[i][j].SumMasterNodeValue();
                }
            }
        }

        public bool[] GetActions(){
            bool[] actions = new bool[this.outputs.Length];
            for (int i = 0; i < this.outputs.Length; i++){
                actions[i] = this.outputs[i].GetResult();
            }
            return actions;
        }

        public void SetInputValues(float[] values){
            for (int i = 0; i < this.inputs.Length; i++){
                inputs[i].SetNodeValue(values[i]);
            }
        }

        public int GetInputsNum(){
            return inputs.Length;
        }

        public int GetOutputsNum(){
            return outputs.Length;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomataConversion
{
    public class NFA
    {
        private List<State> states = null;
        private List<char> symbols = null;
        private State startState = null;
        private List<State> finalStates = null;
        private List<TransitionFunction> transitionFunctions = null;

        public NFA(List<State> states, List<char> symbols, State mStartState, List<State> mFinalStates, List<TransitionFunction> mTransitionFunctions)
        {
            this.states = states;
            this.symbols = symbols;
            startState = mStartState;
            finalStates = mFinalStates;
            transitionFunctions = mTransitionFunctions;
        }

        public NFA(List<State> states, List<char> symbols, State mStartState, State mFinalState, List<TransitionFunction> mTransitionFunctions)
        {
            this.states = states;
            this.symbols = symbols;
            startState = mStartState;
            finalStates = new List<State>();
            finalStates.Add(mFinalState);
            transitionFunctions = mTransitionFunctions;
        }

        public List<State> States
        {
            get { return states; }
            set { states = value; }
        }

        public List<char> Symbols
        {
            get { return symbols; }
            set { symbols = value; }
        }

        public State StartState
        {
            get { return startState; }
            set { startState = value; }
        }

        public List<State> FinalStates
        {
            get { return finalStates; }
            set { finalStates = value; }
        }

        public List<TransitionFunction> TransitionFunctions
        {
            get { return transitionFunctions; }
            set { transitionFunctions = value; }
        }

        public DFA ConvertToDfa()
        {
            //DFA's alphabet is the same as NFA alphabet
            //Also the start state of NFA is the start state for DFA
            List<char> dfaSymbols = this.Symbols;
            State dfaStartState = this.StartState;
            
            //Creating the List for DFA States and adding the start state
            List<State> dfaStates = new List<State>();
            dfaStates.Add(dfaStartState);

            //Creating the List for DFA FinalStates & Transitions
            List<State> dfaFinalStates = new List<State>();
            List<TransitionFunction> dfaTransitions = new List<TransitionFunction>();

            
            Console.WriteLine(" *** SUBSET CONSTRUCTION START ***\n");
            for(int i = 0; i < dfaStates.Count; i++)
            {
                State currentState = dfaStates[i];


                Console.WriteLine("\nCurrent state: " + currentState.ToString() + "\n");

                if (currentState.CheckIfCombined())
                {
                    Console.WriteLine("--Current state is combined, separating its states and continuing--");

                    string[] arr = currentState.Name.Split('.');
                    HashSet<string> reachableStates = new HashSet<string>();

                    foreach (char symbol in dfaSymbols)
                    {
                        Console.WriteLine("************************* Symbol : " + symbol);

                        bool _isFinalState = false;

                        foreach (string t in arr)
                        {
                            if (!string.IsNullOrWhiteSpace(t))
                            {
                                Console.WriteLine("Studying state -{0}- transitions :", t);
                                List<TransitionFunction> res = TransitionFunctions.FindAll(tf => tf.InputState.Name == t && tf.InputSymbol == symbol);

                                Console.WriteLine("### For state {0} there are {1} transitions. ###", t, res.Count);

                                //Gabim
                                if(res.Count == 0)
                                {
                                    continue;
                                }

                                else
                                {
                                    foreach(TransitionFunction r in res)
                                    {
                                        Console.WriteLine("States reachable: -" + r.OutputState.ToString() + "- ");

                                        reachableStates.Add(r.OutputState.Name);

                                        if (FinalStates.Contains(r.OutputState)) _isFinalState = true;
                                    }
                                } 
                            }
                        }

                        if(reachableStates.Count == 0)
                        {
                            Console.WriteLine("!!! CURRENT STATE HAS NO TRANSITIONS TO OTHER STATES ONLY qE(error state) !!!");
                            dfaTransitions.Add(new TransitionFunction(currentState, GetErrorState(dfaStates, dfaTransitions), symbol));
                        }

                        else if(reachableStates.Count == 1)
                        {
                            State tempState = new State(reachableStates.First(), _isFinalState);

                            dfaTransitions.Add(new TransitionFunction(currentState, tempState, symbol));
                            if (!ContainsState(dfaStates, tempState)) dfaStates.Add(tempState);
                            if (!dfaFinalStates.Contains(tempState)) dfaFinalStates.Add(tempState);

                            Console.WriteLine("!!! CURRENT STATE GOES TO ONLY ONE STATE !!!");
                            Console.WriteLine(tempState.ToString());
                        }
                        else
                        {
                            string[] s = new string[reachableStates.Count];

                            reachableStates.CopyTo(s);

                            Array.Sort(s);

                            string stateName = string.Join(".", s);

                            State tempState = new State(stateName, _isFinalState);

                            if (!ContainsState(dfaStates, tempState))
                            {
                                dfaStates.Add(tempState);
                            }

                            if (!dfaFinalStates.Contains(tempState)) dfaFinalStates.Add(tempState);

                            Console.WriteLine("!!! CURRENT STATE GOES TO __COMBINED__ STATES !!!");
                            Console.WriteLine(tempState.ToString());

                            dfaTransitions.Add(new TransitionFunction(currentState, tempState, symbol));

                            reachableStates.Clear();
                        }
                        
                    }
                }

                else
                {
                    Console.WriteLine("Current state in NOT COMBINED!");
                    foreach (char symbol in dfaSymbols)
                    {
                        Console.Write("************************* Symbol : " + symbol);

                        //Search all transitions for given state and symbol
                        List<TransitionFunction> result = TransitionFunctions.FindAll(tf => tf.InputState.Name == currentState.Name && tf.InputSymbol == symbol);

                        if (result.Count == 0)
                        {
                            dfaTransitions.Add(new TransitionFunction(currentState, GetErrorState(dfaStates, dfaTransitions), symbol));

                            Console.WriteLine("!!! CURRENT STATE HAS NO TRANSITIONS TO OTHER STATES ONLY qE(error state)  !!!");
                        }

                        else if (result.Count == 1)
                        {
                            dfaTransitions.Add(new TransitionFunction(currentState, result[0].OutputState, symbol));

                            //Nqs gjendja qe shkojme me inputin e dhene nuk ndodhet ne listen e gjendjeve te DFA atehere e shtojme ate
                            if (!ContainsState(dfaStates, result[0].OutputState))
                                dfaStates.Add(result[0].OutputState);

                            if (FinalStates.Contains(result[0].OutputState) && !dfaFinalStates.Contains(result[0].OutputState)) dfaFinalStates.Add(result[0].OutputState);

                            Console.WriteLine("!!! CURRENT STATE GOES TO ONLY ONE STATE !!!");
                            Console.WriteLine(result[0].OutputState.ToString());
                        }

                        else
                        {
                            string name = null;
                            bool _isFinal = false;
                            bool _isCombined = true;

                            for(int index = 0; index < result.Count; index++)
                            {
                                TransitionFunction tmp = result[index];

                                if(index == result.Count - 1)
                                    name = name + tmp.OutputState.Name;
                                else
                                    name = name + tmp.OutputState.Name + ".";

                                if (finalStates.Contains(tmp.OutputState)) _isFinal = true;
                            }

                            State tempState = new State(name, _isFinal, _isCombined);

                            dfaTransitions.Add(new TransitionFunction(currentState, tempState, symbol));

                            if (!ContainsState(dfaStates, tempState)) dfaStates.Add(tempState);
                            if (_isFinal && !dfaFinalStates.Contains(tempState)) dfaFinalStates.Add(tempState);

                            Console.WriteLine("!!! CURRENT STATE GOES TO __COMBINED__ STATES !!!");
                            Console.WriteLine(tempState.ToString());
                        }
                    }
                }
            }

            Console.WriteLine("\n *** SUBSET CONSTRUCTION ENDED ***\n");

            return new DFA(dfaStates, dfaSymbols, dfaStartState, dfaFinalStates, dfaTransitions);
        }

        private bool IsCombinedStateFinal(List<State> listOfStates, State s)
        {
            if (s.IsCombined)
            {
                s.Name.Trim();
                string[] arr = s.Name.Split('.');
                foreach(string str in arr)
                {
                    State st = new State(str);
                    if (FinalStates.Contains(st)) return true;
                }

                return false;
            }

            return false;
        }

        private bool ContainsState(List<State> listOfStates, State s)
        {
            if (listOfStates.Count == 0)
                return false;
            
            foreach(State tmp in listOfStates)
            {
                if (tmp.CheckIfCombined() && s.CheckIfCombined())
                {
                    string[] s1 = tmp.Name.Split('.');
                    string[] s2 = s.Name.Split('.');

                    if (CheckEqualArrays(s1, s2))
                    {
                        return true;
                    }
                }

                else if (!tmp.CheckIfCombined() && !s.CheckIfCombined())
                {
                    if (tmp.Name == s.Name)
                        return true;
                }
            }
            return false;
        }

        private bool CheckEqualArrays(string[] arr1, string[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;
            else
            {
                Array.Sort(arr1);
                Array.Sort(arr2);

                for (int i = 0; i < arr1.Length; i++)
                {
                    if (arr1[i] != arr2[i])
                        return false;
                }
            }
            return true;
        }

        private State GetErrorState(List<State> dfaStates, List<TransitionFunction> dfaTransitions)
        {
           
            var trapState = dfaStates.Find(state => state.Name == "qE");
            if (trapState != null)
                return trapState;
            else
            {
                var trapSt = new State("qE");
                foreach (var symbol in Symbols)
                {
                    dfaTransitions.Add(new TransitionFunction(trapSt,trapSt,symbol));
                }
                dfaStates.Add(trapSt);
                return trapSt;
            }
        }

        public override string ToString()
        {
            return "Printing out NFA info ...\n\n" 
                + "States: " + Print(States) + "\n"
                + "Symbols: " + Print(Symbols) + "\n"
                + "StartState: " + StartState.ToString() + "\n"
                + "FinalStates: " + Print(FinalStates) + "\n"
                + "Transitions: " + Print(TransitionFunctions) + "\n";
        }


        private string Print(IList list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                object o = list[i];

                if (i == (list.Count - 1))
                    sb.Append(o.ToString()); // Append string to StringBuilder
                else
                    sb.Append(o.ToString()).Append(" & ");
            }
            return sb.ToString(); // Get string from StringBuilder

        }
    }
}
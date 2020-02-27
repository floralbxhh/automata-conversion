using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomataConversion
{
    public class DFA
    {
        private List<State> states = null;
        private List<char> symbols = null;
        private State startState = null;
        private List<State> finalStates = null;
        private List<TransitionFunction> transitionFunctions = null;

        public DFA(List<State> states, List<char> symbols, State startstate, List<State> finalstates,
            List<TransitionFunction> transitionfunctions)
        {
            this.states = states;
            this.symbols = symbols;
            startState = startstate;
            finalStates = finalstates;
            transitionFunctions = transitionfunctions;
        }

        public DFA(List<State> states, List<char> symbols, State startstate, State finalstate,
            List<TransitionFunction> transitionfunctions)
        {
            this.states = states;
            this.symbols = symbols;
            startState = startstate;
            finalStates = new List<State>();
            finalStates.Add(finalstate);
            transitionFunctions = transitionfunctions;
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

        public override string ToString()
        {

            return "Printing out DFA info ...\n\n"

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataConversion
{
    public class ENFAtoNFA
    {
        private ENFA enfa = null;
        private const char EPSILON = 'ε';

        public ENFAtoNFA(ENFA enfa)
        {
            this.enfa = enfa;
        }

        public ENFA Enfa
        {
            get { return enfa; }
        }

        public NFA ConvertToNfa()
        {
            List<State> nfaStates = enfa.States;
            List<char> nfaSymbols = new List<char>();
            foreach(char c in enfa.Symbols)
            {
                if (c != EPSILON) nfaSymbols.Add(c);
            }

            State nfaStartState = enfa.StartState;
            List<State> nfaFinalStates = enfa.FinalStates;
            List<TransitionFunction> nfaTransitionFunctions = new List<TransitionFunction>();

            State errorState = new State("qE", false);


            Console.WriteLine("\n*** Converting ENFA to NFA ***\n\n");
            for(int i = 0; i < nfaStates.Count; i++)
            {
                
                State currentState = nfaStates[i];

                if (currentState != errorState)
                {
                    Console.WriteLine("Current state : " + currentState.ToString());
                    Console.Write("Step 1) Getting ECLOSURE of current state_ : ");
                    List<State> eclosureCurrentState = GetEclosureFromState(currentState, enfa.TransitionFunctions);

                    foreach (State tmp in eclosureCurrentState) Console.Write(tmp.ToString());

                    Console.WriteLine("\nStep 2) Getting reachable states with each symbol from the eclosure set");
                    foreach (char c in nfaSymbols)
                    {
                        Console.WriteLine("*********** Symbol : " + c);
                        HashSet<State> reachableFromCurrent = GetReachableStates(eclosureCurrentState, enfa.TransitionFunctions, c);

                        Console.WriteLine("Reachable States: ");
                        foreach (State _st in reachableFromCurrent) Console.Write(_st.ToString() + " ");

                        if (reachableFromCurrent.Count >= 1)
                        {
                            Console.WriteLine("\n Step 3) Applying eclosure on reachable states");

                            List<State> eclosureOfReachable = EClosureOfStates(reachableFromCurrent, enfa.TransitionFunctions);

                            foreach (State _st2 in eclosureOfReachable) Console.Write("\n" + _st2.ToString() + " ");


                            Console.WriteLine("Step 4) Creating transitions from current state -{0}- to reachable states with symbol '{1}'", currentState.ToString(), c);

                            List<TransitionFunction> tempTransitions = CreateTransitions(currentState, eclosureOfReachable, c);

                            foreach (TransitionFunction tfTmp in tempTransitions) Console.WriteLine(tfTmp.ToString());

                            nfaTransitionFunctions.AddRange(tempTransitions);
                        }

                        else
                        {
                            Console.WriteLine("......Current state -{0}- goes to qE(Error State( with symbol '{1}'", currentState.ToString(), c);
                            if (!nfaStates.Contains(errorState)) nfaStates.Add(errorState);

                            nfaTransitionFunctions.Add(new TransitionFunction(currentState, errorState, c));
                        }
                    }
                }
            }

            Console.WriteLine("\n *** Convertion End ***\n\n");

            return new NFA(nfaStates, nfaSymbols, nfaStartState, nfaFinalStates, nfaTransitionFunctions);
        }

        private List<State> GetEclosureFromState(State s, List<TransitionFunction> transitionList)
        {
            List<State> eclosureList = new List<State>() { s };

            for(int i = 0; i < eclosureList.Count; i++)
            {
                State tmpState = eclosureList[i];

                foreach (TransitionFunction tf in transitionList)
                {
                    if (tf.InputState == tmpState && tf.InputSymbol == EPSILON && !eclosureList.Contains(tf.OutputState)) eclosureList.Add(tf.OutputState);
                }
            }
            
            return eclosureList;
        }

        private HashSet<State> GetReachableStates(List<State> list, List<TransitionFunction> transitionList, char c)
        {
            HashSet<State> reachableStates = new HashSet<State>();

            foreach(State s in list)
            {
                foreach (TransitionFunction tf in transitionList)
                {
                    if (tf.InputState == s && tf.InputSymbol == c) reachableStates.Add(tf.OutputState);
                }
            }

            return reachableStates;
        }

        private List<State> EClosureOfStates(HashSet<State> list, List<TransitionFunction> transitionList)
        {
            List<State> eclosureListofStates = new List<State>(list.ToList<State>());

            for(int i = 0; i < eclosureListofStates.Count; i++)
            {
                State tmpState = eclosureListofStates[i];
                
                foreach(TransitionFunction tf in transitionList)
                {
                    if (tf.InputState == tmpState && tf.InputSymbol == EPSILON && !eclosureListofStates.Contains(tf.OutputState)) eclosureListofStates.Add(tf.OutputState);
                }
            }
            return eclosureListofStates;
        }

        private List<TransitionFunction> CreateTransitions(State s, List<State> reachable, char c)
        {
            List<TransitionFunction> tf = new List<TransitionFunction>();

            foreach(State tmp in reachable)
            {
                tf.Add(new TransitionFunction(s, tmp, c));
            }

            return tf;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataConversion
{
    public class DfaMinimization
    {
        private DFA dfa = null;

        public DfaMinimization(DFA dfa)
        {
            this.dfa = dfa;
        }

        public DFA Dfa
        {
            get { return dfa; }
        }

        public DFA GetMinimizedDfa()
        {
            Console.WriteLine("\n *** FILLIMI I ALGORITMIT TE MINIMIZIMIT ***\n\n");

            Console.WriteLine("HAPI 1 - Fshijme Gjendjet e paarritshme");
            List<State> deletedStates = DeleteNonReachable(Dfa.TransitionFunctions, Dfa.StartState, Dfa.Symbols);

            Console.WriteLine(" 1 --- Printim i gjendjeve aktuale : ");
            foreach(State stmp in deletedStates)
            {
                Console.Write(stmp.ToString() + " ");
            }
            Console.Write("\n");
            Console.WriteLine("Bjendje aktuale sasia: " + deletedStates.Count);

            //Shikoj nqs me jane hequr gjendje te paarritshme, nese po modifikoj dhe listen e kalimeve
            //bool removedStates = !deletedStates.All(Dfa.States.Contains);

            List<TransitionFunction> deletedTransitions = Dfa.TransitionFunctions;

            //if (removedStates)
            //{
                Console.WriteLine("HAPI 2 - Fshijme kalimet ne rast se kemi fshire gjendje");
                deletedTransitions = DeleteTransitions(Dfa.TransitionFunctions, deletedStates, Dfa.Symbols);
            //}


            int statesCount = deletedStates.Count; 

            bool[,] table = new bool[statesCount, statesCount];

            //Creating minimized DFA tuple
            List<State> minDfaStates = new List<State>();
            List<char> minDfaSymbols = Dfa.Symbols;
            State minDfaStartState = Dfa.StartState;
            List<State> minDfaFinalStates = new List<State>();
            List<TransitionFunction> minDfaTransitionFunctions = new List<TransitionFunction>();


            Console.WriteLine("HAPI 3 - Krijohet matrica dhe behet iteracioni per shenjimin e cifteve te gjendjeve");
            //Marking primary diagonal and cells where one of the states is final and the other is non-final
            for(int i = 0; i < statesCount; i++)
            {
                for(int j = 0; j < statesCount; j++)
                {
                    if (i == j)
                    {
                        table[i, j] = true;
                    }

                    else
                    {
                        if ((deletedStates[i].IsFinal && !deletedStates[j].IsFinal) || (!deletedStates[i].IsFinal && deletedStates[j].IsFinal))
                        {
                            table[i, j] = true;
                        }

                        else
                        {
                            table[i, j] = false;
                        }
                    }
                }
            }

            bool equalIteration = false;

            bool[,] oldTable = new bool[statesCount, statesCount];
            //oldTable = table.Clone() as bool[,];

            int iteracion = 1;
            while (!equalIteration)
            {
                Console.WriteLine("///////////////////////////////////////////////////////");
                Console.WriteLine("................................... duke bredhur matricen........... ITERACIONI {0}", iteracion);
                Console.WriteLine("///////////////////////////////////////////////////////");

                oldTable = table.Clone() as bool[,];

                for (int i = 0; i < statesCount; i++)
                {
                    //Console.WriteLine("i : " + i);
                    for (int j = 0; j < i; j++)
                    {
                        //Console.WriteLine("j : " + j);
                        foreach (char s in Dfa.Symbols)
                        {
                            List<State> reachableStates = GetReachableStates(deletedStates[i], deletedStates[j], s, deletedTransitions);

                            if (reachableStates.Count == 2)
                            {
                                //Console.WriteLine(" YEAP JANE 2 GJENDJE TE ARRITSHME ");
                                State s1 = reachableStates[0];
                                //Console.WriteLine("gjendja pare qe po studiohet : " + s1.ToString());

                                State s2 = reachableStates[1];
                                //Console.WriteLine("gjendja pare qe po studiohet : " + s2.ToString());

                                int k = FindIndexOfState(s1, deletedStates);

                                //Console.WriteLine("Indeksi i gjendjes {0} ne deletedStates eshte : {1}",s1.ToString(), k);

                                int l = FindIndexOfState(s2, deletedStates);

                                //Console.WriteLine("Indeksi i gjendjes {0} ne deletedStates eshte : {1}", s2.ToString(), l);

                                if (k != 999 && l != 999)
                                {
                                    if (table[k, l] == true)
                                    {
                                        table[i, j] = true;
                                        table[j, i] = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error couldn't find state {0} or {1} in dfa states list.", s1.ToString(), s2.ToString());
                                }
                            }
                        }
                    }
                }

                //Last step check if old table is equal to actual table
                Console.WriteLine("\n\n!!!!!!!!!! SHIKOJ A KAM TABELA TE NJEJTA !!!!!!!!!!!!!!\n");
                if (AreTableEqual(oldTable, table))
                {
                    equalIteration = true;
                    Console.WriteLine(" ######################### iterimi perfundoi u arriten tabela te njejta");
                }
                else
                {
                    Console.WriteLine("\n\n NUK KEMI TABELA TE NJEJTA. ITERIMI VAZHDON\n");
                    iteracion++;
                }
            }

            
            //Shikoj nqs kam qeliza tabele me vlere buleane false atehere kemi nje minimizim te mundshem
            if (CheckIfNoMarkedCell(table))
            {
                Console.WriteLine("@@@@ HAPI 4 - U gjeten qeliza jo te shenjuara, fillon algoritmi i krijimit te DFA te minimizuar @@@");

                //Saving the unmarked state in a list of type state
                //Saving combined states in a list of type list<state>
                List<State> unmarkedStates = new List<State>();
                List<State> markedStates = new List<State>();

                List<List<State>> listCombinedStates = new List<List<State>>();

                List<State> combinedStates = new List<State>();

                for (int i = 0; i < table.GetLength(0); i++)
                {
                    for(int j = 0; j < i; j++)
                    {
                        //Nqs qeliza ka vlere buleane false atehere shtoj gjendjet perkatese ne keto indekse ne listen e gjendjeve jo te markuara
                        if(!table[i, j])
                        {
                            if(!unmarkedStates.Contains(deletedStates[i])) unmarkedStates.Add(deletedStates[i]);
                            if(!unmarkedStates.Contains(deletedStates[j])) unmarkedStates.Add(deletedStates[j]);

                            List<State> tmp = new List<State>();
                            tmp.Add(deletedStates[i]);
                            tmp.Add(deletedStates[j]);

                            listCombinedStates.Add(tmp);
                        }
                    }
                }


                //Vendos ne gjendjet e markuara ne tabelen e gjendjeve te shenuara
                for (int i = 0; i < table.GetLength(0); i++)
                {
                    if (!unmarkedStates.Contains(deletedStates[i])) markedStates.Add(deletedStates[i]);
                }

                Console.WriteLine("..... Printim vlerash per listat :");
                Console.WriteLine("1) Lista e gjendjeve te pashenjuara : [ ");
                foreach(State stmp in unmarkedStates)
                {
                    Console.Write(stmp.ToString() + " ");
                }
                Console.Write(" ]\n");

                Console.WriteLine("2) Lista e gjendjeve te shenjuara : [ ");
                foreach (State stmp in markedStates)
                {
                    Console.Write(stmp.ToString() + " ");
                }
                Console.Write(" ]\n");

                Console.WriteLine("3) Gjendjet qe kombinohen : [");
                foreach(List<State> ltmp in listCombinedStates)
                {
                    Console.Write("[ ");
                    foreach(State stmp in ltmp)
                    {
                        Console.Write(stmp.ToString() + " ");
                    }
                    Console.Write(" ], ");
                }
                Console.Write(" ]\n");

                //Nqs lista qe mban listen e gjendjeve ekuivalente ka me shume se 1 shikoj nese arrij nje bashkim mes ketyre listave
                if (listCombinedStates.Count > 1)
                {
                    listCombinedStates = JoinListCombinedStates(listCombinedStates);

                    Console.WriteLine("\n\n !!! SHIKOJ A MUND TE LIDH ME SHUME GJENDJE BASHKE !!!!!!");

                    Console.WriteLine("3.1) Gjendjet e lidhura bashke: [");
                    foreach (List<State> ltmp in listCombinedStates)
                    {
                        Console.Write("[ ");
                        foreach (State stmp in ltmp)
                        {
                            Console.Write(stmp.ToString() + " ");
                        }
                        Console.Write(" ], ");
                    }
                    Console.Write(" ]\n");
                }

                //Per cdo liste gjendjesh te kombinuara krijoj gjendjen e re emri i te ciles jane emrat e ketyre gjendjeve  te bashkuara me ','
                for(int i = 0; i < listCombinedStates.Count; i++)
                {
                   List<State> ls = listCombinedStates[i];

                    String str = String.Join(".", ls);

                    bool checkIfFinal = false;
                    
                    for(int j = 0; j < ls.Count; j++)
                    {
                        if (dfa.FinalStates.Contains(ls[j])) checkIfFinal = true;
                    }

                    State combinedTmp = new State(str, checkIfFinal);

                    //Shikoj nqs combinedTmp eshte finale e fus ne listen e gjendjeve finale te DFA_MIN
                    if (checkIfFinal)
                        minDfaFinalStates.Add(combinedTmp);

                    //Shikoj nqs lista aktuale permban gjendjen fillestare te DFA
                    //Nqs e permban dmth qe gjendja fillestare e DFA_MIN eshte nje gjendje e kombinuar
                    if (ls.Contains(dfa.StartState))
                        minDfaStartState = combinedTmp;

                    combinedStates.Add(combinedTmp);
                }

                Console.WriteLine("4) Gjendjet e kombinuara te bashkuara bashke : [ ");
                foreach(State stmp in combinedStates)
                {
                    Console.WriteLine(stmp.ToString() + " ");
                }
                Console.WriteLine(" ]");

                //Krijoj gjendjet e DFA_MIN
                minDfaStates.AddRange(markedStates);
                minDfaStates.AddRange(combinedStates);

                //Shikoj nqs gjendjet finale te DFA jane te markuara dhe i vendos ato ne gjendjet finale te DFA_MIN
                foreach(State _s in Dfa.FinalStates)
                {
                    if (markedStates.Contains(_s)) minDfaFinalStates.Add(_s);
                }

                //Tani ripunoj listen e kalimeve per cdo gjendje
                Console.WriteLine("\n\n PERPUNIMI I LISTES SE KALIMEVE {startState} -> {simbol} -> {outState}\n");
                foreach(TransitionFunction transition in deletedTransitions)
                {
                    foreach (char s in minDfaSymbols)
                    {
                        if(transition.InputSymbol == s)
                        {
                            Console.Write("Kalimi : " + transition.ToString());
                            //4 raste kalimesh
                            //Rasti 1) gjendje vetem -> gjendje vetem
                            if(markedStates.Contains(transition.InputState) && markedStates.Contains(transition.OutputState))
                            {
                                Console.Write(" gjendje vetem -> gjendje vetem\n\n");
                                //E shtoj pasi nuk kam nevoj per modifikim
                                minDfaTransitionFunctions.Add(transition);
                            }

                            //Rasti 2) gjendje vetem -> gjendje kombinuar
                            else if(markedStates.Contains(transition.InputState) && unmarkedStates.Contains(transition.OutputState))
                            {
                                State _tmp = GetCombinedStateOfState(transition.OutputState, combinedStates);

                                if(_tmp != null)
                                {
                                    Console.Write(" gjendje vetem -> gjendje kombinuar\n\n");
                                    TransitionFunction tmpTransition = new TransitionFunction(transition.InputState, _tmp, s);

                                    if (!minDfaTransitionFunctions.Contains(tmpTransition)) minDfaTransitionFunctions.Add(tmpTransition);
                                }
                            }

                            //Rasti 3) gjendje kombinuar -> gjendje vetem
                            else if(unmarkedStates.Contains(transition.InputState) && markedStates.Contains(transition.OutputState))
                            {
                                State _tmp = GetCombinedStateOfState(transition.InputState, combinedStates);

                                if(_tmp != null)
                                {
                                    Console.Write(" gjendje kombinuar -> gjendje vetem\n\n");
                                    TransitionFunction tmpTransition = new TransitionFunction(_tmp, transition.OutputState, s);

                                    if (!minDfaTransitionFunctions.Contains(tmpTransition)) minDfaTransitionFunctions.Add(tmpTransition);
                                }
                            }

                            //Rasti 4) gjendje kombinuar -> gjendje kombinuar
                            else
                            {
                                State _tmp1 = GetCombinedStateOfState(transition.InputState, combinedStates);
                                State _tmp2 = GetCombinedStateOfState(transition.OutputState, combinedStates);

                                if(_tmp1 != null && _tmp2 != null)
                                {
                                    Console.Write(" gjendje kombinuar -> gjendje kombinuar\n\n");
                                    TransitionFunction tmpTransition = new TransitionFunction(_tmp1, _tmp2, s);

                                    if (!minDfaTransitionFunctions.Contains(tmpTransition)) minDfaTransitionFunctions.Add(tmpTransition);
                                }
                            }
                        }
                    }
                }
            }

            else
            {
                Console.WriteLine("Nuk u gjend minimizim!!!");

                minDfaStates = deletedStates;
                minDfaFinalStates = Dfa.FinalStates;
                minDfaTransitionFunctions = deletedTransitions;

            }

            return new DFA(minDfaStates, minDfaSymbols, minDfaStartState,minDfaFinalStates, minDfaTransitionFunctions);
        }


        private List<State> DeleteNonReachable(List<TransitionFunction> transitionsList, State start, List<char> alphabet)
        {
            List<State> reached = new List<State>();
            reached.Add(start);

            for(int i = 0; i < reached.Count; i++)
            {
                State current = reached[i];
                foreach(char s in alphabet)
                {
                    foreach(TransitionFunction tf in transitionsList)
                    {
                        if(tf.InputSymbol == s && (tf.InputState.ToString() == current.ToString()))
                        {
                            if (!reached.Contains(tf.OutputState)) reached.Add(tf.OutputState);
                        }
                    }
                }
            }

            //reached.Sort();

            return reached;
        }

        private List<TransitionFunction> DeleteTransitions(List<TransitionFunction> transitionsList, List<State> states, List<char> alphabet)
        {
            List<TransitionFunction> newTf = new List<TransitionFunction>();

            foreach(char s in alphabet)
            {
                foreach (TransitionFunction tf in transitionsList)
                {
                    if(tf.InputSymbol == s)
                    {
                        if(states.Contains(tf.InputState) && states.Contains(tf.OutputState))
                        {
                            if (!newTf.Contains(tf)) newTf.Add(tf);
                        }
                    }
                }
            }

            return newTf;
        }


        private List<State> GetReachableStates(State s1, State s2, char c, List<TransitionFunction> transitions)
        {
            List<State> reachableStates = new List<State>();

            foreach(TransitionFunction tf in transitions)
            {
                if((tf.InputState == s1 && tf.InputSymbol == c) || (tf.InputState == s2 && tf.InputSymbol == c))
                {
                    if (!reachableStates.Contains(tf.OutputState)) reachableStates.Add(tf.OutputState);
                }
            }

            return reachableStates;
        }

        private bool AreTableEqual(bool[,] t1, bool[,] t2)
        {
            bool equal = true;

            if(t1.GetLength(0) == t2.GetLength(0))
            {
                for (int i = 0; i < t1.GetLength(0); i++)
                {
                    for (int j = 0; j < t1.GetLength(0); j++)
                    {
                        if (t1[i, j] != t2[i, j])
                        {
                            equal = false;
                            break;
                        }
                    }
                    if (!equal) break;
                }
            }
            else { Console.WriteLine("GJATESITE E MATRICAVE TE NDRYSHME !");return false; }
            return equal;
        }

        private int FindIndexOfState(State s, List<State> list)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].Name == s.Name) return i;
            }

            return 999;
        }

        private bool CheckIfNoMarkedCell(bool[,] table)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for(int j = 0; j < i; j++)
                {
                    if (table[i, j] == false)
                        return true;
                }
            }

            return false;
        }

        private List<List<State>> JoinListCombinedStates(List<List<State>> l)
        {
           for(int i = 0; i < l.Count; i++)
            {
                for(int j = 0; j < l.Count; j++)
                {
                    if(i != j)
                    {
                        if(CheckCommonState(l[i], l[j]))
                        {
                            l[i].AddRange(l[j]);

                            l[i] = l[i].Distinct().ToList();

                            l.Remove(l[j]);

                        }
                    }
                }
            }

            return l;
        }

        private bool CheckCommonState(List<State> l1, List<State> l2)
        {
            for(int i = 0; i < l1.Count; i++)
            {
                for(int j = 0; j < l2.Count; j++)
                {
                    if (l1[i] == l2[j]) return true;
                }
            }
            return false;
        }

        private List<TransitionFunction> GetTransitionsNonCombinedStates(List<TransitionFunction> dfaTransitions, List<State> nonCombinedStates, char c)
        {
            List<TransitionFunction> tmp = new List<TransitionFunction>();

            foreach(TransitionFunction tf in dfaTransitions)
            {
                if(tf.InputSymbol == c && nonCombinedStates.Contains(tf.InputState) && nonCombinedStates.Contains(tf.OutputState))
                {
                    if (!tmp.Contains(tf))
                        tmp.Add(tf);
                }

            }

            return tmp;
        }

        private State GetCombinedStateOfState(State s, List<State> combinedStates)
        {
            for (int i = 0; i < combinedStates.Count; i++)
            {
                State current = combinedStates[i];

                string[] arr = current.Name.Split('.');

                for(int j = 0; j < arr.GetLength(0); j++)
                {
                    if (s.Name == arr[j])
                        return current;
                }
            }

            return null;
        }
    }
}

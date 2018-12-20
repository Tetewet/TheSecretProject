using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public  class AIUIDialogues 
{
    
    public string AIUIDialogueGrunt(List<AIInfoClass> infoClass)
    {

        Random random = new Random();
        int randomInt = random.Next(0, 10);
        string textToShow = "";
        switch (infoClass[0].Distancefoe < infoClass[0].MyMovePoints)
        {
            case true:
                switch (randomInt)
                {
                    case 1:
                        textToShow = "I'm close enough!";
                        break;
                    case 2:
                        textToShow = "It's too late!";
                        break;
                    case 3:
                        textToShow = "I can see you " + infoClass[0].Target.Name + " !";
                        break;
                    case 4:
                        textToShow = "You better stay";
                        break;
                    case 5:
                        textToShow = "I'll destroy you";
                        break;
                    case 6:
                        textToShow = "Into the fray!";
                        break;
                    case 7:
                        textToShow = "Forward!";
                        break;
                    case 8:
                        textToShow = "No mercy!";
                        break;
                    case 9:
                        textToShow = "Onward to victory!";
                        break;
                    case 10:
                        textToShow = "It will hurt!";
                        break;

                    default:
                        textToShow = "Charge!!!";
                        break;
                }
                break;
            case false:
                switch (randomInt)
                {
                    case 1:
                        textToShow = "Die! all of you!";
                        break;
                    case 2:
                        textToShow = "Bastards!";
                        break;
                    case 3:
                        textToShow = "Meet your maker!";
                        break;
                    case 4:
                        textToShow = "Your end is nigh!";
                        break;
                    case 5:
                        textToShow = "For the kill!";
                        break;
                    case 6:
                        textToShow = "I will enjoy this";
                        break;
                    case 7:
                        textToShow = "Fool!";
                        break;
                    case 8:
                        textToShow = "Show no mercy!";
                        break;
                    case 9:
                        textToShow = "Arghhh!";
                        break;
                    case 10:
                        textToShow = "Die " + infoClass[0].Target.Name + " !!!";
                        break;

                    default:
                        textToShow = "Charge!!!";
                        break;
                }
                break;

                
        }
        if (infoClass[0].Threathened)
        {
            switch (randomInt)
            {
                case 1:
                    textToShow = "RUN!!!!";
                    break;
                case 2:
                    textToShow = "Fleee!";
                    break;
                case 3:
                    textToShow = "Not Again!";
                    break;
                case 4:
                    textToShow = "Enough!";
                    break;
                case 5:
                    textToShow = "Spare me " + infoClass[0].Target.Name +" !";
                    break;
                case 6:
                    textToShow = "I won't enjoy this!";
                    break;
                case 7:
                    textToShow = "I'm sorry!";
                    break;
                case 8:
                    textToShow = "Show mercy!";
                    break;
                case 9:
                    textToShow = "I beg of you!";
                    break;
                case 10:
                    textToShow = "Let me go!";
                    break;

                default:
                    textToShow = "Too strong!!!";
                    break;
            }
        }



        
        return textToShow;
    }
}


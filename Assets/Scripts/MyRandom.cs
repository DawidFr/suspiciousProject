using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Special{
    public static class MyRandom{
    
             

        //return random object from array or list;
        public static T GetObject<T>(T[] array) {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
        public static T GetObject<T>(T[,] array, bool x, int pos) {
            return x? array[pos , UnityEngine.Random.Range(0, array.Length)]: array[UnityEngine.Random.Range(0, array.Length), pos];
        }
        public static T GetObject<T>(List<T> list){
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        public static string GetMaleName(){
            List<string> nameList = new List<string>(){"Jordan","Ezio","Mark","Michel","Omar","Steven","David","John","Ahmed","George","Mike","Leonardo","Thomas","Tom","Nathan"};
            return GetObject<string>(nameList);


        }
        public static string GetFeamleName(){
            List<string> nameList = new List<string>(){"Olivia","Jessica","Emma","Michele","Isabella","Sophia","Mia","Amelia","Charlotte","Amber","Violet"};
            return GetObject<string>(nameList);


        }
        public static string GetName(){
            return Random.Range(0,11) < 2? GetFeamleName():GetMaleName(); 
        }
        public static int GetFromVector(Vector2Int vector2){
            return Random.Range(vector2.x , vector2.y);
        }
        public static float GetFromVector(Vector2 vector2){
            return Random.Range(vector2.x , vector2.y);
        }
        public static bool GetRandomBool(){
            return Random.Range(0,2) == 1;  
        }
    
    
    
   
   
   
   
   
    }
}


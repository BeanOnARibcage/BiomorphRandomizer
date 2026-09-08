using Il2CppLanguage.Lua;
using System.Collections.Generic;

namespace BiomorphRandomizer;

public static class TableHandler {
	
	public static LuaTable ListToTable(List<long> list) {
		LuaTable table = new LuaTable();
		int n = list.Count;
		for (int i = 0; i < n; i++) {
			table.AddRaw(i.ToString(), new LuaNumber(list[i]));
		}
		return table;
	}
	
	public static List<long> TableToList(LuaTable table) {
		List<long> list = new List<long>();
		while (table.Count == 0 && table.Length == 1) {
			table = table.GetValue(1).Cast<LuaTable>();
		}
		int n = table.Count;
		for (int i = 0; i < n; i++) {
			list.Add((long)table.GetValue(i.ToString()).Cast<LuaNumber>().Number);
		}
		return list;
	}
	
	public static LuaTable DictToTable(Dictionary<long, long> dict) {
		LuaTable table = new LuaTable();
		foreach (KeyValuePair<long, long> pair in dict) {
			table.AddRaw(pair.Key.ToString(), new LuaNumber(pair.Value));
		}
		return table;
	}
	
	public static Dictionary<long, long> TableToDict(LuaTable table) {
		Dictionary<long, long> dict = new Dictionary<long, long>();
		// Data that goes in as one table comes out nested in another table
		LuaTable table2 = table.GetValue(1).Cast<LuaTable>();
		
		Il2CppSystem.Collections.Generic.Dictionary<LuaValue, LuaValue> tableDict = table2.Dict;
		foreach (Il2CppSystem.Collections.Generic.KeyValuePair<LuaValue, LuaValue> pair in tableDict) {
			string key = pair.Key.Cast<LuaString>().Text;
			long val = (long)pair.Value.Cast<LuaNumber>().Number;
			dict.Add(long.Parse(key), val);
		}
		return dict;
	}	
}
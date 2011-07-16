<Query Kind="Statements">
  <Reference Relative="..\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll">&lt;Personal&gt;\d\GitHub\Rx-Demo\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll</Reference>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

var rnd = new Random();
var qry = Enumerable.Range(1,5)
		  .Select(ii=> new {Index=ii, Random = rnd.Next()});

qry.Dump();
qry.Where(item => item.Index ==1).Dump();
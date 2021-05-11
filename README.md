# jupiter
OPC UA Client

![EXAMPLE](https://github.com/bamchoh/jupiter/blob/images/example.gif "example")

# Natural Sort algorythm

This application is using Natural Sort algorythm to sort variable node tree.
I'm using the algorythm which is describing in below web site:
http://final.hateblo.jp/entry/2015/08/16/212631

I'd like to thank you @finalstream for the greatest post.

# Clear Script Support

```js
// test comments
function dump(vals)
{
  let getType = function(val) {
    return val.WrappedValue.TypeInfo.BuiltInType.ToString();
  }

  for(let i = 0;i < vals.Count;i++)
  {
    let str = '';
    str += `status code: ${vals[i].StatusCode.ToString()}, `;
    str += `value: ${client.DataValueToString(vals[i])}, `;
    str += `type: ${getType(vals[i])}`;
    utils.WriteLine(str);
  }
}

let vars = [
  "ns=2;s=Scalar_Static_Boolean",
  "ns=2;s=Scalar_Static_SByte",
  "ns=2;s=Scalar_Static_Byte",
  "ns=2;s=Scalar_Static_Int16",
  "ns=2;s=Scalar_Static_Int32",
  "ns=2;s=Scalar_Static_Int64",
];

client.Write([{"node_id": "ns=2;s=Scalar_Static_Boolean", "value": false}]);
client.Write([{"node_id": "ns=2;s=Scalar_Static_SByte", "value": 127}]);
client.Write([{"node_id": "ns=2;s=Scalar_Static_Byte", "value": 255}]);
client.Write([{"node_id": "ns=2;s=Scalar_Static_Int16", "value": 32767}]);
client.Write([{"node_id": "ns=2;s=Scalar_Static_Int32", "value": 0x7FFFFFFF}]);
client.Write([{"node_id": "ns=2;s=Scalar_Static_Int64", "value": 0x7FFFFFFFFFFFFFFFn}]);

for(let i = 0;i < 3;i++)
{
  let readResults = client.Read(vars);
  dump(readResults);
  let writeVals = [];
  for(let i = 0;i < readResults.Count;i++)
  {
    writeVals.push({
      "node_id": vars[i],
      "value": client.TryToBigInt(readResults[i]) + 1n,
    });
  }
  let results = client.Write(writeVals);
}
```

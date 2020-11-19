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
    str += `value: ${vals[i].Value}, `;
    str += `type: ${getType(vals[i])}`;
    utils.WriteLine(str);
  }
}

let vars = [
  "ns=2;s=Scalar_Static_Byte",
  "ns=2;s=Scalar_Static_Int16",
  "ns=2;s=Scalar_Static_Int32",
];

for(let i = 0;i < 100;i++)
{
  let readResults = client.Read(vars);
  dump(readResults);
  let writeVals = [];
  for(let i = 0;i < readResults.Count;i++)
  {
    writeVals.push({
      "node_id": vars[i],
      "value": readResults[i].Value + 1,
    });
  }
  let results = client.Write(writeVals);
}

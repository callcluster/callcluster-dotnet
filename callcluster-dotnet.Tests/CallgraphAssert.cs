using System;
using System.Linq;
using callcluster_dotnet.dto;
using callcluster_dotnet.Tests;
using Xunit;
public class CallgraphAssert : Assert
{
    public static void CallPresent(CallgraphDTO dto,string from, string to){
        long fromIndex = Utils.IndexOf(dto,from);
        long toIndex = Utils.IndexOf(dto,to);
        Assert.Contains(new CallDTO(){ from = fromIndex, to = toIndex },dto.calls,Utils.CallComparer);
    }

    internal static void CallsFrom(CallgraphDTO dto, string function, int quantity=1)
    {
        long fromIndex = Utils.IndexOf(dto,function);
        Assert.True(dto.calls.Where(c=>c.from==fromIndex).ToList().Count()==quantity);
    }
}
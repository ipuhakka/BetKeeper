function byHighest(data, key)
{
    return data.sort(function (a, b) {
			return compareNumber(b[key], a[key]);
    });
}

function byLowest(data, key){
  return data.sort(function (a,b){
    return compareNumber(a[key], b[key]);
  })
}

/*
 Sorts the array in ascending or descending order.
*/
function byRank(data, key, sortByHighest){

  if (sortByHighest){
    return byHighest(data, key);
  }

  return byLowest(data, key);
}

/*
  Sorts and array alphabetically.
*/
function alphabetically(data, key, sortOrderNormal){

  if (sortOrderNormal){
  	return data.sort(function(a, b){
      return compareString(b[key], a[key]);
  	});
  }
    return data.sort(function(a, b){
      return compareString(a[key], b[key]);
  })
}

function compareNumber(a, b){
  return a - b;
}

/*
  Compares strings:
  Returns 0 if values are equal, -1
  if a is bigger and 1 if b is bigger.
*/
function compareString(a, b){

  a = a.toLowerCase();
  b = b.toLowerCase();

  const aBigger = a > b;

  if (aBigger){
    return -1;
  }

  const bBigger = b > a;

  return bBigger ? 1 : 0;
}

module.exports = {
  byRank,
  byHighest,
  alphabetically
};

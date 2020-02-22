import consts from './consts';
import moment from 'moment';

/**
 * Formats a utc datetime to local time string.
 * @param {*} momentValue 
 */
export function formatDateTime(momentValue)
{
  return moment.utc(momentValue).local().format(consts.DATETIME_FORMAT);
}
/*
  Shallow equality comparison.
*/
export function shallowEquals(obj, compare){

  for (var key in obj)
  {
    if (!(key in compare) || obj[key] !== compare[key])
    {
      return false;
    }
  }

  for (var key2 in compare)
  {
    if (!(key2 in obj) || obj[key2] !== compare[key2])
    {
      return false;
    }
  }

  return true;
}

/*
  Checks if number is odd
*/
export function isOdd(number){
  	return (number % 2) === 1;
}

/*
  Checks that value is valid double.
*/
export function isValidDouble(value){

  if (value.length === 0 ||
      value[0] === '.' ||
      value[value.length - 1] === '.')
  {
    return false;
  }

  if (Number.isNaN(value)){
    return false;
  }

  return true;
}

/*
  Checks a string only contains a letter, number, or character
  '-', '_', '.'. Returns true if is a valid string.
*/
export function isValidString(value){
  var nonAllowed = /[!@#$%^&*()+=[\]{};'':"\\|,<>/?]/;

  return !nonAllowed.test(value);
}

/* Deep copies an object. */
export function deepCopy(obj){
  return JSON.parse(JSON.stringify(obj));
}

export function loginCredentialsExist(){
  return sessionStorage.getItem('token') != null &&
    sessionStorage.getItem('token').toString() !== 'null';
}

export function isInteger(value)
{
  return /^\+?(0|[1-9]\d*)$/.test(value);
}

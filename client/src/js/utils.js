import consts from './consts';
import moment from 'moment';
import _ from 'lodash';

/** Converts a camel cased string into a string from which each capital letter starts a new word.
 * First word is capitalized.
 */
export function camelCaseToText(camelCasedString)
{
  if (_.isNil(camelCasedString)
    ||camelCasedString.length === 0 
    || typeof camelCasedString !== 'string')
  {
    return '';
  }

  let text = camelCasedString.charAt(0).toUpperCase() + camelCasedString.slice(1);

  let asText = ''; 

  _.forEach(text, (char, index) =>
  {
    if (char === char.toUpperCase() && index !== 0)
    {
      asText += ` ${char.toLowerCase()}`;
    }
    else
    {
      asText += char;
    }
  });

  return asText;
}

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

/**
 * Returns response text either into a json object or as plain text.
 * @param {object} response 
 */
export function getResponseBody(response)
{
  return response.getResponseHeader('content-type').includes('application/json')
    ? JSON.parse(response.responseText)
    : response.responseText;
}

/**
 * Returns key's value from data.
 * Returns null if data is not an object, or no match is found.
 * @param {object} data 
 * @param {string} key 
 */
export function findNestedValue(data, key)
{
  if (!isObject(data))
  {
    return null;
  }

  if (!_.isNil(data[key]))
  {
    return data[key];
  }

  const keys = Object.keys(data);

  for (let i = 0; i < keys.length; i++)
  {
    const value = findNestedValue(data[keys[i]], key);

    if (!_.isNil(value))
    {
      return value;
    }
  }

  return null;
}

/**
 * Checks if value is object.
 * @param {*} value 
 */
export function isObject(value)
{
  return value && typeof value === 'object' && value.constructor === Object;
}

/**
 * Creates an object from entries.
 * @param {array} entries 
 */
export function fromEntries(entries)
{
  return Object.assign({}, ...Array.from(entries, ([key, value]) => ({[key]: value}) ));
}

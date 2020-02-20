import ConstVars from '../consts';

/** Get page structure. */
export function getPage(pathname)
{
    return new Promise(function(resolve, reject){
        var xmlHttp = new XMLHttpRequest();
    
        xmlHttp.onreadystatechange =( () => {
          if (xmlHttp.readyState === 4)
          {
            if (xmlHttp.status === 200)
            {
              resolve(xmlHttp);
            }
            else 
            {
              reject(xmlHttp);
            }
          }
        });
    
        // Remove '/' char from start
        xmlHttp.open("GET", `${ConstVars.URI}${pathname.substr(1)}`);
        xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.setRequestHeader('Content-type', 'application/json');

        xmlHttp.send();
      });
}
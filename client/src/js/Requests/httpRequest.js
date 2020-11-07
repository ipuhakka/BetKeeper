import consts from '../consts';
import _ from 'lodash';

class HttpRequest 
{
    /**
     * Constructs a HttpRequest instance
     * @param {string} url 
     * @param {string} method 
     * @param {array} headers Key value pairs
     * @param {*} data 
     */
    constructor(url, method, headers, data)
    {
        this.url = url;
        this.method = method;
        this.headers = headers;
        this.data = data;
    }

    /**
     * Sends a request and resolves on getting a success response
     */
    sendRequest()
    {
        const { url, method, headers, data } = this;

        return new Promise(function(resolve, reject)
        {
            var xmlHttp = new XMLHttpRequest();

            xmlHttp.onreadystatechange = (() => 
            {
                if (xmlHttp.readyState === 4)
                {
                    if ([200, 201, 204].includes(xmlHttp.status))
                    {
                        resolve(xmlHttp);
                    }
                    else 
                    {
                        reject(xmlHttp);
                    }
                }
            });

            xmlHttp.open(method, consts.URI + url);

            _.forEach(headers, header => 
            {
                xmlHttp.setRequestHeader(header.key, header.value);
            });

            // Data ignored with GET-requests, okay to add always
            xmlHttp.send(data);
        });
    }
}

export default HttpRequest;
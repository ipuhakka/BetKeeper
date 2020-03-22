import * as pageUtils from '../../src/js/pageUtils';

var chai = require('chai');
var expect = chai.expect;
var _ = require('lodash');

const tabPage = {
    key: 'page',
    components: [
        { 
            key: 'tab1',
            componentType: 'Tab',
            tabContent: [
                {
                    key: "name",
                    dataKey: 'dataKey1'
                },
                {
                    key: "name2",
                    dataKey: 'dataKey2.dataKey2Nested2'
                }
            ] 
        },
        { 
            key: 'tab2',
            componentType: 'Tab',
            tabContent: [
                {
                    key: "name3",
                    dataKey: 'dataKey3'
                },
                {
                    key: "name4",
                }
            ] 
        }
    ]
}

const componentPage = {
    key: 'page',
    components: [
        {
            key: "name",
            dataKey: 'dataKey1'
        },
        {
            key: "name2",
            dataKey: 'dataKey2.dataKey2Nested2'
        }
    ]
}

const pageData = {
    dataKey1: 'dataValue',
    dataKey2: {
        dataKey2Nested1: 1,
        dataKey2Nested2: true
    },
    dataKey3: null,
    dataKey4: 'test'
};

describe('findComponentFromPage', function()
{
    it('Finds component from tabs', function(done)
    {
        expect(pageUtils.findComponentFromPage(tabPage, "name2").key).to.equal('name2');
        done();
    });

    it('Finds component from regular page', function(done)
    {
        expect(pageUtils.findComponentFromPage(componentPage, "name2").key).to.equal('name2');
        done();
    });

    it('Returns null when page not found', function(done)
    {
        expect(pageUtils.findComponentFromPage(componentPage, "unexisting key")).to.equal(null);
        expect(pageUtils.findComponentFromPage(tabPage, "unexisting key")).to.equal(null);
        done();
    })
});

describe('getDataFromComponents', function()
{
    it('Returns object with all data matching dataKeys in tab page', function(done)
    {
        const result = pageUtils.getDataFromComponents(tabPage.components, pageData);

        expect(Object.keys(result).length).to.equal(3);
        
        // object keys are component keys
        expect(result['name']).to.equal('dataValue');
        expect(result['name2']).to.equal(true);
        expect(result['name3']).to.equal(null);

        done();
    });

    it('Returns object with all data matching dataKeys in component page', function(done)
    {
        const result = pageUtils.getDataFromComponents(componentPage.components, pageData);

        expect(Object.keys(result).length).to.equal(2);
        
        // object keys are component keys
        expect(result['name']).to.equal('dataValue');
        expect(result['name2']).to.equal(true);

        done();
    });
});
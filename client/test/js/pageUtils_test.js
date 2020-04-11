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

describe('replaceComponent', function()
{
    const tabPage = {
        key: 'page',
        components: [
            { 
                key: 'tab1',
                componentType: 'Tab',
                tabContent: [
                    {
                        key: "name",
                        dataKey: 'dataKey1',
                        componentType: 'Container',
                        children: [
                            { key: 'child1'}
                        ]
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

    it('replaces component on tab page', function(done)
    {
        const newContainer = { 
            key: 'name', 
            componentType: 'Container', 
            children: [
            { key: 'newChild' } 
        ]};

        const components = pageUtils.replaceComponent(tabPage, newContainer);

        expect(components[0].tabContent[0].children[0].key).to.equal('newChild');
        done();
    });

    it('replaces component on normal page', function(done)
    {
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

        const newComponent = { 
            key: 'name', 
            dataKey: 'dataKey2'
        };

        const components = pageUtils.replaceComponent(componentPage, newComponent);

        expect(components[0].dataKey).to.equal('dataKey2');
        done();
    });
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
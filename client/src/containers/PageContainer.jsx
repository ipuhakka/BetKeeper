import React, { Component } from 'react';
import * as pageActions from '../actions/pageActions';
import Page from '../views/Page/Page';

class PageContainer extends Component 
{
    constructor(props)
    {
        super(props);
        
        this.state = {
            pageKey: ''
        }
    }

    componentDidMount()
    {
        // TODO: Aseta pageKey tilaan pathnamen perusteella. 
        // Katso myös sisältääkö pages proppi jo oikean rakenteen, tällöin hae vain data.
        pageActions.getPage(window.location.pathname);
    }

    render()
    {
        return <Page />;
    }
};

// TODO: Page propsit mukaan

export default PageContainer;
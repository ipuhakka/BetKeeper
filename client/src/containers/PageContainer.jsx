import React, { Component } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import _ from 'lodash';
import { withRouter } from 'react-router-dom';
import * as pageActions from '../actions/pageActions';
import Page from '../views/Page/Page';

class PageContainer extends Component 
{   
    constructor(props)
    {
        super(props);

        this.state = {
            pathname: window.location.pathname
        };
    }

    /**
     * Load page structure 
     */
    componentDidMount()
    {
        if (_.isNil(this.props.page))
        {
            pageActions.getPage(window.location.pathname);
        }
    }

    componentDidUpdate()
    {
        const { state } = this;

        if (state.pathname !== window.location.pathname)
        {
            pageActions.getPage(window.location.pathname);
         
            this.setState({ pathname: window.location.pathname });
        }
    }

    render()
    {
        return <Page {...this.props.page}/>;
    }
};

const mapStateToProps = (state) => 
{
    const pathname = window.location.pathname;
    // Parse page name from path
    const pageKey = pathname.substring(pathname.lastIndexOf('/') + 1);

    return {
        page: _.find(state.pages.pages, page =>
            page.key === pageKey)
    };
};

PageContainer.propTypes = {
    page: PropTypes.shape({
        key: PropTypes.string.isRequired,
        // TODO: shape these
        components: PropTypes.array.isRequired,
        data: PropTypes.object.isRequired
    })
};

export default withRouter(connect(mapStateToProps)(PageContainer));
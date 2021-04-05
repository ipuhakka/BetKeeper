import React, { Component } from 'react';
import PropTypes from 'prop-types';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import './Page.css';

class Panel extends Component
{
    constructor(props)
    {
        super(props);

        this.state = {
            visible: true
        }
    }

    renderPanelContent()
    {
        if (!this.state.visible)
        {
            return null;
        }

        return <div className='page-panel-content listGroupItemBody visible'>
                {this.props.children}
            </div>; 
    }

    render()
    {
        return <div>
            <ListGroupItem
                className='page-panel-header'
                onClick={() => 
                {
                    this.setState({visible: !this.state.visible});
                }}>
                {this.props.legend}
                <div className={`arrow-div arrow ${this.state.visible ? 'down' : 'right'} `} />
            </ListGroupItem>
            {this.state.visible && this.renderPanelContent()}
        </div>;
    }
};

Panel.propTypes = {
    legend: PropTypes.string
}

export default Panel;
import React, { Component } from 'react';
import PropTypes from 'prop-types';
import './Page.css';

class DynamicColorText extends Component 
{
    render()
    {
        return <div>
        {
            this.props.items.map((item, i) => {
                const text = `${item.value}${i < this.props.items.length - 1 ? ',' : ''} `;
                return <span key={`dynamic-color-text-${item}-${i}`} className={`dynamic-color-text ${item.color}`}>{text}</span>;
            })
        }
        </div>;
    }

};

DynamicColorText.propTypes = {
    items: PropTypes.arrayOf(
        PropTypes.exact({
            value: PropTypes.string.isRequired,
            color: PropTypes.string.isRequired
        })
    )
};

export default DynamicColorText;
import React, { Component } from 'react';
import PropTypes from 'prop-types';
import RBButton from 'react-bootstrap/Button';
import './Button.css';

class Button extends Component
{

    constructor(props)
    {
        super(props);

        this.onClick = this.onClick.bind(this);
    }

    onClick()
    {
        const { props } = this;

        if (props.buttonType === 'PageAction')
        {
            props.onClick(
                props.action, 
                props.actionDataKeys, 
                props.requireConfirm, 
                props.componentsToInclude, 
                props.text, 
                props.style, 
                props.navigateTo);
        }
        else if (props.buttonType === 'ModalAction')
        {
            props.onClick(props.action, props.components, props.text, props.requireConfirm, props.style);
        }
        else if (props.buttonType === 'Navigation')
        {
            props.onClick(props.navigateTo);
        }
    }

    render()
    {
        const { props } = this;

        return <RBButton
            className='button'
            variant={props.style}
            onClick={this.onClick}>
                {props.text}
            </RBButton>
    }
};

Button.defaultProps = {
    requireConfirm: false
}

Button.propTypes = {
    buttonType: PropTypes.oneOf(['ModalAction', 'PageAction', 'Navigation']).isRequired,
    text: PropTypes.string.isRequired,
    style: PropTypes.string.isRequired,
    action: PropTypes.string,
    actionDataKeys: PropTypes.arrayOf(PropTypes.string),
    components: PropTypes.array,
    navigateTo: PropTypes.string,
    onClick: PropTypes.func.isRequired,
    requireConfirm: PropTypes.bool.isRequired,
    componentsToInclude: PropTypes.arrayOf(PropTypes.string)
};

export default Button;
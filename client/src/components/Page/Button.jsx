import React, { Component } from 'react';
import PropTypes from 'prop-types';
import RBButton from 'react-bootstrap/Button';
import './Page.css';

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
                props.navigateTo,
                props.staticData);
        }
        else if (props.buttonType === 'ModalAction')
        {
            props.onClick(
                props.action,
                props.components,
                props.text,
                props.requireConfirm,
                props.style,
                props.absoluteDataPath,
                props.componentsToInclude);
        }
        else if (props.buttonType === 'Navigation')
        {
            props.onClick(props.navigateTo);
        }
    }

    render()
    {
        const { props } = this;

        if (props.displayType === 'Icon')
        {
            return <RBButton 
                className={`page-image-button ${props.className}`} 
                variant={props.buttonStyle}
                onClick={this.onClick}>
            <i className={props.iconName}></i>
          </RBButton>;
        }

        return <RBButton
            className={`button ${props.className}`} 
            variant={props.buttonStyle}
            disabled={props.requireValidFields && props.invalidFields}
            onClick={this.onClick}>
                {props.text}
            </RBButton>;
    }
};

Button.defaultProps = {
    requireConfirm: false
}

Button.propTypes = {
    buttonType: PropTypes.oneOf(['ModalAction', 'PageAction', 'Navigation']).isRequired,
    text: PropTypes.string.isRequired,
    buttonStyle: PropTypes.string.isRequired,
    action: PropTypes.string,
    actionDataKeys: PropTypes.arrayOf(PropTypes.string),
    /** Data set in server side */
    staticData: PropTypes.object,
    components: PropTypes.array,
    navigateTo: PropTypes.string,
    onClick: PropTypes.func.isRequired,
    requireConfirm: PropTypes.bool.isRequired,
    componentsToInclude: PropTypes.arrayOf(PropTypes.string),
    displayType: PropTypes.oneOf(['Text', 'Icon']).isRequired,
    absoluteDataPath: PropTypes.string,
    iconName: function(props, propName) 
    {
        if (props['displayType'] === 'Icon' && typeof(props[propName]) !== 'string') 
        {
            return new Error('iconName prop invalid or missing');
        }
    },
    className: PropTypes.string,
    invalidFields: PropTypes.bool,
    requireValidFields: PropTypes.bool
};

export default Button;
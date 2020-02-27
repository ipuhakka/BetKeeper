import React, { Component } from 'react';
import _ from 'lodash';
import { withRouter } from 'react-router-dom';
import * as pageActions from '../../actions/pageActions';
import Header from '../../components/Header/Header';
import Menu from '../../components/Menu/Menu';
import Button from '../../components/Page/Button';
import Field from '../../components/Page/Field';
import Modal from '../../components/Page/Modal';
import Table from '../../components/Page/Table';
import Info from '../../components/Info/Info.jsx';
import './Page.css';

class Page extends Component
{
    constructor(props)
    {
		super(props);

		this.state = {
            actionModalOpen: false,
            actionModalProps: {},
            data: {}
        };
        
        this.clickModalAction = this.clickModalAction.bind(this);
        this.clickNavigationButton = this.clickNavigationButton.bind(this);
        this.clickPageAction = this.clickPageAction.bind(this);
        this.onDataChange = this.onDataChange.bind(this);
    }

    onDataChange(key, newValue)
    {
        this.setState({
            data: {
                ...this.state.data,
                [key]: newValue
            }
        });
    }

    clickPageAction(action, actionDataKeys)
    {
        const { state, props } = this;

        const parameters = {};

        _.forEach(actionDataKeys, dataKey => 
        {
            if (!_.isNil(state.data[dataKey]))
            {
                parameters[dataKey] = state.data[dataKey];
            }
            // parameter in page data
            else 
            {
                parameters[dataKey] = props.data[dataKey];
            }
        });

        const pathname = window.location.pathname;
        // Parse page name from path
        const pageKey = pathname.split('page/')[1];

        pageActions.callAction(pageKey, action, parameters);
    }

    clickModalAction(action, actionFields, title)
    {
        this.setState({
            actionModalOpen: true,
            actionModalProps: {
                action,
                actionFields,
                title
            }
        });
    }

    clickNavigationButton(navigateTo)
    {
        this.props.history.push(navigateTo);
    }

    /**
     * Returns function for button click event handling.
     * @param {object} button 
     */
    getButtonClick(button)
    {
        if (button.buttonType === 'ModalAction')
        {
            return this.clickModalAction;
        }

        if (button.buttonType === 'PageAction')
        {
            return this.clickPageAction;
        }

        if (button.buttonType === 'Navigation')
        {
            return this.clickNavigationButton;
        }

        return null;
    }

    renderComponents(components, classname)
    {
        return <div key={classname} className={classname}>
            {_.map(components, (component, i) => 
            {
                switch (component.componentType)
                {
                    case 'Container':
                        return this.renderComponents(component.children, 'container-div');

                    case 'Button':
                        return <Button 
                            key={`button-${component.action}`}
                            onClick={this.getButtonClick(component)} 
                            {...component} />;

                    case 'Field':
                        return <Field 
                            onChange={this.onDataChange}
                            key={`field-${component.key}`} 
                            type={component.fieldType} 
                            fieldKey={component.key}
                            initialValue={_.get(this.props.data, component.dataKey, '')} 
                            {...component} />;

                    case 'Table':
                        return <Table onRowClick={this.clickNavigationButton} key={`itemlist-${i}`} {...component} />;

                    default:
                        throw new Error(`Component type ${component.componentType} not implemented`);
                }
            })}
        </div>;
    }
    
    render()
    {
        const { state, props } = this;

        const pathname = window.location.pathname;
        // Parse page name from path and convert first char to upper
        let pageKey = pathname
            .substring(pathname.lastIndexOf('/') + 1);

        pageKey = pageKey
            .charAt(0)
            .toUpperCase() + pageKey.slice(1);

        return <div className="content">
            <Modal 
                onClose={() => 
                {
                    this.setState({ actionModalOpen: false });
                }} 
                page={pageKey}
                show={state.actionModalOpen} 
                {...state.actionModalProps}/>
            <Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disableValue={pageKey}></Menu>
            <Info alertState={state.alertState} alertText={state.alertText} dismiss={this.dismissAlert}></Info>
            {this.renderComponents(props.components, 'page-content')}
        </div>;
    }
};

export default withRouter(Page);
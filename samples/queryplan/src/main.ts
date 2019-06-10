/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the Source EULA. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
'use strict';

import * as azdata from 'azdata';
import * as vscode from 'vscode';

let toggleOn: boolean = false;
let statusView = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right);
statusView.text = 'Query Plan watcher on';

export function activate(context: vscode.ExtensionContext) {
	vscode.commands.registerCommand('queryplan.ToggleProcessPlan', () => {
		toggleOn = !toggleOn;
		if (toggleOn) {
			statusView.show();
		} else {
			statusView.hide();
		}
	});

	azdata.queryeditor.registerQueryEventListener({
		onQueryEvent(type: azdata.queryeditor.QueryEvent, document: azdata.queryeditor.QueryDocument, args: any) {
			if (type === 'executionPlan') {
				if (toggleOn) {
					let tab2 = azdata.window.createTab('Query Watcher 2');
					tab2.registerContent(async view => {
						let webview1 = view.modelBuilder.webView()
							.withProperties({
								html: "<h1>Here it is</h1>"
							}).component();

						webview1.onMessage((params) => {
							webview1.html = '<h2>Updated Content</h2>';
						});

						let divModel = view.modelBuilder.divContainer()
							.withItems([webview1])
							.withLayout({ width: '100%', height: '100%' })
							.component();

						await view.initializeModel(divModel);
					});
					document.createQueryTab(tab2);

					let tab = azdata.window.createTab('Query Watcher');
					tab.registerContent(async view => {
						let fileNameTextBox = view.modelBuilder.inputBox().component();
						let xmlTextBox = view.modelBuilder.inputBox().component();

						let formModel = view.modelBuilder.formContainer()
							.withFormItems([{
								component: fileNameTextBox,
								title: 'File name'
							}, {
								component: xmlTextBox,
								title: 'Plan XML'
							}]).withLayout({ width: '100%' }).component();

						await view.initializeModel(formModel);

						fileNameTextBox.value = document.uri;
						xmlTextBox.value = <string>args;
					});

					document.createQueryTab(tab);
				}
			}
		}
	});
}

// this method is called when your extension is deactivated
export function deactivate(): void {
}

export function escape(html: string): string {
	return html.replace(/[<|>|&|"]/g, function (match) {
		switch (match) {
			case '<': return '&lt;';
			case '>': return '&gt;';
			case '&': return '&amp;';
			case '"': return '&quot;';
			case '\'': return '&#39';
			default: return match;
		}
	});
}
